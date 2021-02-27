// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandHandler : IRequestHandler<
        ValidateAppMfaCodeAgainstCurrentUserCommand, Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler> _logger;

        public ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
            IUserRepository userRepository,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._logger = logger;
        }

        public async Task<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Handle(
            ValidateAppMfaCodeAgainstCurrentUserCommand request,
            CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Process(
            ValidateAppMfaCodeAgainstCurrentUserCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var authApp = user.AuthenticatorApps.SingleOrDefault(x => x.WhenRevoked == null);

            if (authApp == null)
            {
                this._logger.LogDebug("Auth app not found.");
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.NoAuthenticatorAppEnrolled));
            }

            var secretBytes = Base32Encoding.ToBytes(authApp.Key);
            var topt = new Totp(secretBytes);
            var isVerified = topt.VerifyTotp(request.Code, out _);

            if (isVerified)
            {
                return Result.Ok<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ValidateAppMfaCodeAgainstCurrentUserCommandResult(user.Id));
            }

            this._logger.LogDebug("Code not valid.");
            return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                new ErrorData(ErrorCodes.MfaCodeNotValid));
        }
    }
}
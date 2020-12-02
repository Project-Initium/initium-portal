// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandHandler : IRequestHandler<ValidateEmailMfaCodeAgainstCurrentUserCommand, Result<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IClock _clock;
        private readonly ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler> _logger;

        public ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(
            IUserRepository userRepository, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            IClock clock, ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
            this._logger = logger;
        }

        public async Task<Result<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Handle(ValidateEmailMfaCodeAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<Result<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Process(ValidateEmailMfaCodeAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (!currentUserMaybe.HasValue || !(currentUserMaybe.Value is UnauthenticatedUser currentUser))
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUser.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            var totp = new Totp(user.SecurityStamp.ToByteArray());
            var result = totp.VerifyTotp(request.Code, out var _, new VerificationWindow(3, 3));
            if (!result)
            {
                this._logger.LogDebug("Code not valid.");
                user.ProcessPartialSuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    AuthenticationHistoryType.EmailMfaFailed);
                return Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.MfaCodeNotValid));
            }

            user.ProcessSuccessfulAuthenticationAttempt(this._clock.GetCurrentInstant().ToDateTimeUtc());
            return Result.Ok<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(user.Id));
        }
    }
}
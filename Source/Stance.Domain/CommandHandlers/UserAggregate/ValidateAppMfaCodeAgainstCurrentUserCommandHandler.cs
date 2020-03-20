// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OtpNet;
using ResultMonad;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandHandler : IRequestHandler<
        ValidateAppMfaCodeAgainstCurrentUserCommand, Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;

        public ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
            IUserRepository userRepository,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        public async Task<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Handle(
            ValidateAppMfaCodeAgainstCurrentUserCommand request,
            CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>> Process(
            ValidateAppMfaCodeAgainstCurrentUserCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var authApp = user.AuthenticatorApps.SingleOrDefault(x => x.WhenRevoked == null);

            if (authApp == null)
            {
                return Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.AuthenticatorAppAlreadyEnrolled));
            }

            var secretBytes = Base32Encoding.ToBytes(authApp.Key);
            var topt = new Totp(secretBytes);
            var isVerified = topt.VerifyTotp(request.Code, out _);

            return isVerified
                ? Result.Ok<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ValidateAppMfaCodeAgainstCurrentUserCommandResult(user.Id))
                : Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.MfaCodeNotValid));
        }
    }
}
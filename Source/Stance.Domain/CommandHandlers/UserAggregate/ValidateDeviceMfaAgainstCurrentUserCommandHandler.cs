// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class ValidateDeviceMfaAgainstCurrentUserCommandHandler : IRequestHandler<
        ValidateDeviceMfaAgainstCurrentUserCommand, Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;

        public ValidateDeviceMfaAgainstCurrentUserCommandHandler(IUserRepository userRepository, IFido2 fido2,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IClock clock)
        {
            this._userRepository = userRepository;
            this._fido2 = fido2;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
        }

        public async Task<Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>> Handle(
            ValidateDeviceMfaAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>> Process(
            ValidateDeviceMfaAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe =
                await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            var authenticatorDevice = user.AuthenticatorDevices.FirstOrDefault(x =>
                request.AuthenticatorAssertionRawResponse.Id.SequenceEqual(x.CredentialId));

            if (authenticatorDevice == null)
            {
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.DeviceNotFound));
            }

            var res = await this._fido2.MakeAssertionAsync(
                request.AuthenticatorAssertionRawResponse,
                request.AssertionOptions, authenticatorDevice.PublicKey, (uint)authenticatorDevice.Counter,
                @params => Task.FromResult(true));

            authenticatorDevice.UpdateCounter((int)res.Counter, this._clock.GetCurrentInstant().ToDateTimeUtc());

            return Result.Ok<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                new ValidateDeviceMfaAgainstCurrentUserCommandResult(user.Id, res));
        }
    }
}
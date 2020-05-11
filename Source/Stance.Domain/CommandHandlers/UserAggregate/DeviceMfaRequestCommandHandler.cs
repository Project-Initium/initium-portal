// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Helpers;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class DeviceMfaRequestCommandHandler : IRequestHandler<DeviceMfaRequestCommand,
        Result<DeviceMfaRequestCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;

        public DeviceMfaRequestCommandHandler(
            IUserRepository userRepository,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IClock clock, IFido2 fido2)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
            this._fido2 = fido2;
        }

        public async Task<Result<DeviceMfaRequestCommandResult, ErrorData>> Handle(
            DeviceMfaRequestCommand request,
            CancellationToken cancellationToken)
        {
            var result = await this.Process(cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<DeviceMfaRequestCommandResult, ErrorData>> Process(
            CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var optionsMaybe = this._fido2.GenerateAssertionOptionsForUser(user);

            if (!optionsMaybe.HasValue)
            {
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.FidoVerificationFailed));
            }

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                AuthenticationHistoryType.AppMfaRequested);
            return Result.Ok<DeviceMfaRequestCommandResult, ErrorData>(
                new DeviceMfaRequestCommandResult(optionsMaybe.Value));
        }
    }
}
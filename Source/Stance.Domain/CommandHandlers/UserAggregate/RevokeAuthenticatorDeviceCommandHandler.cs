// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class
        RevokeAuthenticatorDeviceCommandHandler : IRequestHandler<RevokeAuthenticatorDeviceCommand,
            ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;

        public RevokeAuthenticatorDeviceCommandHandler(
            IUserRepository userRepository,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IClock clock)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            RevokeAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<ResultWithError<ErrorData>> Process(
            RevokeAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUser.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.AuthenticatorDevices.All(x => x.Id != request.DeviceId))
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.DeviceNotFound));
            }

            user.RevokeAuthenticatorDevice(request.DeviceId, this._clock.GetCurrentInstant().ToDateTimeUtc());
            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
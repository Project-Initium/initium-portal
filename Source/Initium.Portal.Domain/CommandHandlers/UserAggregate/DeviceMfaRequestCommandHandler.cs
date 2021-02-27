// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class DeviceMfaRequestCommandHandler : IRequestHandler<DeviceMfaRequestCommand,
        Result<DeviceMfaRequestCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeviceMfaRequestCommandHandler> _logger;

        public DeviceMfaRequestCommandHandler(
            IUserRepository userRepository, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            IClock clock, IFido2 fido2, ILogger<DeviceMfaRequestCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
            this._fido2 = fido2;
            this._logger = logger;
        }

        public async Task<Result<DeviceMfaRequestCommandResult, ErrorData>> Handle(
            DeviceMfaRequestCommand request,
            CancellationToken cancellationToken)
        {
            var result = await this.Process(cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<Result<DeviceMfaRequestCommandResult, ErrorData>> Process(
            CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var optionsResult = this._fido2.GenerateAssertionOptionsForUser(user);

            if (optionsResult.IsFailure)
            {
                this._logger.LogError(optionsResult.Error, "No Fido options.");
                return Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.FidoVerificationFailed));
            }

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                AuthenticationHistoryType.AppMfaRequested);
            return Result.Ok<DeviceMfaRequestCommandResult, ErrorData>(
                new DeviceMfaRequestCommandResult(optionsResult.Value));
        }
    }
}
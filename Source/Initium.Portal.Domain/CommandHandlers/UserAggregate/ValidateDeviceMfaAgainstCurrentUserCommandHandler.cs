// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class ValidateDeviceMfaAgainstCurrentUserCommandHandler : IRequestHandler<
        ValidateDeviceMfaAgainstCurrentUserCommand, Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ValidateDeviceMfaAgainstCurrentUserCommandHandler> _logger;

        public ValidateDeviceMfaAgainstCurrentUserCommandHandler(IUserRepository userRepository, IFido2 fido2,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IClock clock,
            ILogger<ValidateDeviceMfaAgainstCurrentUserCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._fido2 = fido2;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
            this._logger = logger;
        }

        public async Task<Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>> Handle(
            ValidateDeviceMfaAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>> Process(
            ValidateDeviceMfaAgainstCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe =
                await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            var authenticatorDevice = user.AuthenticatorDevices.FirstOrDefault(x =>
                request.AuthenticatorAssertionRawResponse.Id.SequenceEqual(x.CredentialId));

            if (authenticatorDevice == null)
            {
                this._logger.LogDebug("No auth device found.");
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
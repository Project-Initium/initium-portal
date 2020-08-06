// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
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
    public class
        EnrollAuthenticatorDeviceCommandHandler : IRequestHandler<EnrollAuthenticatorDeviceCommand, Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public EnrollAuthenticatorDeviceCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IFido2 fido2, ILogger<EnrollAuthenticatorDeviceCommandHandler> logger)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._fido2 = fido2 ?? throw new ArgumentNullException(nameof(fido2));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>> Handle(
            EnrollAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));

        }

        private async Task<Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>> Process(
            EnrollAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe =
                await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            Fido2.CredentialMakeResult credentialMakeResult;
            try
            {
                Task<bool> IsCredentialIdUniqueToUser(IsCredentialIdUniqueToUserParams param)
                {
                    var count = user.AuthenticatorDevices.Count(x =>
                        x.CredentialId == param.CredentialId && !x.IsRevoked);

                    return Task.FromResult(count == 0);
                }

                credentialMakeResult = await this._fido2.MakeNewCredentialAsync(
                    request.AuthenticatorAttestationRawResponse,
                    request.CredentialCreateOptions,
                    IsCredentialIdUniqueToUser);
            }
            catch (Fido2VerificationException ex)
            {
                this._logger.LogError(ex, "Error with cred req.");
                return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.FidoVerificationFailed));
            }

            var device = user.EnrollAuthenticatorDevice(
                Guid.NewGuid(),
                whenHappened,
                credentialMakeResult.Result.PublicKey,
                credentialMakeResult.Result.CredentialId,
                credentialMakeResult.Result.Aaguid,
                Convert.ToInt32(credentialMakeResult.Result.Counter),
                request.Name,
                credentialMakeResult.Result.CredType);

            return Result.Ok<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new EnrollAuthenticatorDeviceCommandResult(credentialMakeResult, device.Id));
        }
    }
}
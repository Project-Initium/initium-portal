// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
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
    public class
        EnrollAuthenticatorDeviceCommandHandler : IRequestHandler<EnrollAuthenticatorDeviceCommand, Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;

        public EnrollAuthenticatorDeviceCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IFido2 fido2)
        {
            this._userRepository = userRepository;
            this._clock = clock;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._fido2 = fido2;
        }

        public async Task<Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>> Handle(
            EnrollAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>> Process(
            EnrollAuthenticatorDeviceCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Result.Fail<EnrollAuthenticatorDeviceCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe =
                await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
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
            catch (Fido2VerificationException)
            {
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
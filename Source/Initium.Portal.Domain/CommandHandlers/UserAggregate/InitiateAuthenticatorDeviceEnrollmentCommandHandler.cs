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
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class InitiateAuthenticatorDeviceEnrollmentCommandHandler : IRequestHandler<
        InitiateAuthenticatorDeviceEnrollmentCommand,
        Result<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IFido2 _fido2;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public InitiateAuthenticatorDeviceEnrollmentCommandHandler(
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IUserRepository userRepository,
            IFido2 fido2, ILogger<InitiateAuthenticatorDeviceEnrollmentCommandHandler> logger)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._fido2 = fido2 ?? throw new ArgumentNullException(nameof(fido2));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>> Handle(
            InitiateAuthenticatorDeviceEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return Result.Fail<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe =
                await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var fidoUser = new Fido2User
            {
                Name = user.EmailAddress,
                DisplayName = user.EmailAddress,
                Id = user.Id.ToByteArray(),
            };

            var publicKeyCredentialDescriptors =
                user.AuthenticatorDevices.Where(x => !x.IsRevoked).Select(x => new PublicKeyCredentialDescriptor(x.CredentialId)).ToList();

            var authenticatorSelection = new AuthenticatorSelection
            {
                RequireResidentKey = false,
                UserVerification = UserVerificationRequirement.Preferred,
                AuthenticatorAttachment = request.AuthenticatorAttachment,
            };

            var authenticationExtensionsClientInputs = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationIndex = true,
                Location = true,
                UserVerificationMethod = true,
                BiometricAuthenticatorPerformanceBounds = new AuthenticatorBiometricPerfBounds
                {
                    FAR = float.MaxValue,
                    FRR = float.MaxValue,
                },
            };

            var options = this._fido2.RequestNewCredential(fidoUser, publicKeyCredentialDescriptors,
                authenticatorSelection, AttestationConveyancePreference.None, authenticationExtensionsClientInputs);

            return Result.Ok<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                new InitiateAuthenticatorDeviceEnrollmentCommandResult(options));
        }
    }
}
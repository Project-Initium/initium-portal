// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using Initium.Portal.Domain.Helpers;
using MaybeMonad;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand,
        Result<AuthenticateUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly SecuritySettings _securitySettings;
        private readonly IUserRepository _userRepository;
        private readonly IFido2 _fido2;
        private readonly ILogger _logger;

        public AuthenticateUserCommandHandler(IUserRepository userRepository, IClock clock,
            IOptions<SecuritySettings> securitySettings, IFido2 fido2, ILogger<AuthenticateUserCommandHandler> logger)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._fido2 = fido2 ?? throw new ArgumentNullException(nameof(fido2));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<Result<AuthenticateUserCommandResult, ErrorData>> Handle(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                this._logger.LogDebug("Failed saving changes.");
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<AuthenticateUserCommandResult, ErrorData>> Process(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var maybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (maybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = maybe.Value;

            if (user.IsDisabled)
            {
                user.ProcessUnsuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    false);
                this._logger.LogDebug("User is disabled.");
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.AccountIsDisabled));
            }

            if (!user.IsVerified)
            {
                user.ProcessUnsuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    this._securitySettings.AllowedAttempts != -1 && user.AttemptsSinceLastAuthentication >=
                    this._securitySettings.AllowedAttempts);
                this._logger.LogDebug("User is not verified.");
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.AccountNotVerified));
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.ProcessUnsuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    this._securitySettings.AllowedAttempts != -1 && user.AttemptsSinceLastAuthentication >=
                    this._securitySettings.AllowedAttempts);
                this._logger.LogDebug("Password is not valid.");
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.AuthenticationFailed));
            }

            var authenticationState = BaseAuthenticationProcessCommandResult.AuthenticationState.Unknown;
            var mfaProvider = MfaProvider.None;

            var optionsMaybe = Maybe<AssertionOptions>.Nothing;
            if (user.AuthenticatorDevices.Any(x => x.WhenRevoked == null))
            {
                optionsMaybe = this._fido2.GenerateAssertionOptionsForUser(user);
                if (optionsMaybe.HasValue)
                {
                    user.ProcessPartialSuccessfulAuthenticationAttempt(
                        this._clock.GetCurrentInstant().ToDateTimeUtc(),
                        AuthenticationHistoryType.DeviceMfaRequested);
                    authenticationState =
                        BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaDeviceCode;
                    mfaProvider |= MfaProvider.Device;
                }
            }

            if (user.AuthenticatorApps.Any(x => x.WhenRevoked == null))
            {
                if (authenticationState == BaseAuthenticationProcessCommandResult.AuthenticationState.Unknown)
                {
                    user.ProcessPartialSuccessfulAuthenticationAttempt(
                        this._clock.GetCurrentInstant().ToDateTimeUtc(),
                        AuthenticationHistoryType.AppMfaRequested);
                    authenticationState = BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaAppCode;
                }

                mfaProvider |= MfaProvider.App;
            }

            if (authenticationState == BaseAuthenticationProcessCommandResult.AuthenticationState.Unknown)
            {
                user.ProcessPartialSuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    AuthenticationHistoryType.EmailMfaRequested);
                var totp = new Totp(user.SecurityStamp.ToByteArray());

                var token = totp.ComputeTotp();

                user.AddIntegrationEvent(new EmailMfaTokenGeneratedIntegrationEvent(
                    user.EmailAddress,
                    user.Profile.FirstName,
                    user.Profile.LastName,
                    token));
                authenticationState = BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode;
            }

            mfaProvider |= MfaProvider.Email;

            if (optionsMaybe.HasValue)
            {
                return Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(
                    user.Id,
                    mfaProvider, optionsMaybe.Value));
            }

            return Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(
                user.Id,
                authenticationState,
                mfaProvider));
        }
    }
}
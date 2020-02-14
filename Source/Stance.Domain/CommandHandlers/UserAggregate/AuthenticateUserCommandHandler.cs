// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using NodaTime;
using OtpNet;
using ResultMonad;
using Stance.Core.Constants;
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand,
        Result<AuthenticateUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly SecuritySettings _securitySettings;
        private readonly IUserRepository _userRepository;

        public AuthenticateUserCommandHandler(IUserRepository userRepository, IClock clock,
            IOptions<SecuritySettings> securitySettings)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<Result<AuthenticateUserCommandResult, ErrorData>> Handle(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
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
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = maybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.ProcessUnsuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    this._securitySettings.AllowedAttempts != -1 && user.AttemptsSinceLastAuthentication >=
                    this._securitySettings.AllowedAttempts);
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.AuthenticationFailed));
            }

            if (this._securitySettings.EnforceMfa)
            {
                user.ProcessPartialSuccessfulAuthenticationAttempt(
                    this._clock.GetCurrentInstant().ToDateTimeUtc(),
                    AuthenticationHistoryType.EmailMfaRequested);
                var totp = new Totp(user.SecurityStamp.ToByteArray());

                var generated = totp.ComputeTotp();

                user.AddDomainEvent(new EmailMfaTokenGeneratedEvent(
                    user.Id,
                    user.EmailAddress,
                    generated));

                return Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(
                    user.Id, user.EmailAddress,
                    AuthenticateUserCommandResult.AuthenticationState.AwaitingMfaEmailCode));
            }

            user.ProcessSuccessfulAuthenticationAttempt(this._clock.GetCurrentInstant().ToDateTimeUtc());
            return Result.Ok<AuthenticateUserCommandResult, ErrorData>(
                new AuthenticateUserCommandResult(user.Id, user.EmailAddress));
        }
    }
}
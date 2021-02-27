// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class EmailMfaRequestedCommandHandler : IRequestHandler<EmailMfaRequestedCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<EmailMfaRequestedCommandHandler> _logger;

        public EmailMfaRequestedCommandHandler(
            IUserRepository userRepository, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            IClock clock, ILogger<EmailMfaRequestedCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            EmailMfaRequestedCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var totp = new Totp(user.SecurityStamp.ToByteArray());

            var token = totp.ComputeTotp();

            user.AddIntegrationEvent(new EmailMfaTokenGeneratedIntegrationEvent(
                user.EmailAddress,
                user.Profile.FirstName,
                user.Profile.LastName,
                token));

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                AuthenticationHistoryType.EmailMfaRequested);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public PasswordResetCommandHandler(IUserRepository userRepository, IClock clock, IOptions<SecuritySettings> securitySettings, ILogger<PasswordResetCommandHandler> logger)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<ResultWithError<ErrorData>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));

        }

        private async Task<ResultWithError<ErrorData>> Process(PasswordResetCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var convertedToken = new Guid(Convert.FromBase64String(request.Token));
            var userMaybe =
                await this._userRepository.FindByUserBySecurityToken(convertedToken, whenHappened, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.PasswordHistories.OrderByDescending(x => x.WhenUsed).Take(this._securitySettings.HistoricalLimit)
                .Any(x => BCrypt.Net.BCrypt.Verify(request.NewPassword, x.Hash)))
            {
                this._logger.LogDebug("Password is not valid.");
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.PasswordInHistory));
            }

            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.NewPassword), whenHappened);
            user.CompleteTokenLifecycle(convertedToken, whenHappened);
            user.AddDomainEvent(new PasswordChangedEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName));

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
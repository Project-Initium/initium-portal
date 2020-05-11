// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IClock _clock;

        public PasswordResetCommandHandler(IUserRepository userRepository, IClock clock, IOptions<SecuritySettings> securitySettings)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<ResultWithError<ErrorData>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(PasswordResetCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var convertedToken = new Guid(Convert.FromBase64String(request.Token));
            var userMaybe =
                await this._userRepository.FindByUserBySecurityToken(convertedToken, whenHappened, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.PasswordHistories.OrderByDescending(x => x.WhenUsed).Take(this._securitySettings.HistoricalLimit)
                .Any(x => BCrypt.Net.BCrypt.Verify(request.NewPassword, x.Hash)))
            {
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
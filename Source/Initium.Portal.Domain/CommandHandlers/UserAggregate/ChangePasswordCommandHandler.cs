// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using MediatR;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResultWithError<ErrorData>>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IClock _clock;

        public ChangePasswordCommandHandler(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IUserRepository userRepository, IOptions<SecuritySettings> securitySettings, IClock clock)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._currentAuthenticatedUserProvider =
                currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._securitySettings = securitySettings.Value;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            ChangePasswordCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(
            ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.PasswordNotCorrect, "Password is not correct"));
            }

            if (user.PasswordHistories.OrderByDescending(x => x.WhenUsed).Take(this._securitySettings.HistoricalLimit)
                .Any(x => BCrypt.Net.BCrypt.Verify(request.NewPassword, x.Hash)))
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.PasswordInHistory));
            }

            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.NewPassword), this._clock.GetCurrentInstant().ToDateTimeUtc());
            user.AddDomainEvent(new PasswordChangedEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName));

            this._userRepository.Update(user);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
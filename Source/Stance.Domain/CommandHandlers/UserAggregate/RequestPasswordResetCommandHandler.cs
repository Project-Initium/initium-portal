// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
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
    public class
        RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly SecuritySettings _securitySettings;
        private readonly IUserRepository _userRepository;

        public RequestPasswordResetCommandHandler(IUserRepository userRepository, IClock clock,
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

        public async Task<ResultWithError<ErrorData>> Handle(
            RequestPasswordResetCommand request, CancellationToken cancellationToken)
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
            RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();

            var token = user.GenerateNewPasswordResetToken(
                whenHappened,
                TimeSpan.FromHours(this._securitySettings.PasswordTokenLifetime));

            user.AddDomainEvent(new PasswordResetTokenGeneratedEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName, token));

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
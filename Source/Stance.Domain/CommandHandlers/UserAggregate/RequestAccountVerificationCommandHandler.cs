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

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class
        RequestAccountVerificationCommandHandler : IRequestHandler<RequestAccountVerificationCommand,
            ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly SecuritySettings _securitySettings;
        private readonly IUserRepository _userRepository;

        public RequestAccountVerificationCommandHandler(
            IUserRepository userRepository,
            IOptions<SecuritySettings> securitySettings, IClock clock)
        {
            this._userRepository = userRepository;
            this._clock = clock;
            this._securitySettings = securitySettings.Value;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            RequestAccountVerificationCommand request,
            CancellationToken cancellationToken)
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
            RequestAccountVerificationCommand request,
            CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.IsVerified)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.UserIsAlreadyVerified));
            }

            user.GenerateNewAccountConfirmationToken(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                TimeSpan.FromHours(this._securitySettings.AccountVerificationTokenLifetime));

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
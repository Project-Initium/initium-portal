// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class
        VerifyAccountAndSetPasswordCommandHandler : IRequestHandler<VerifyAccountAndSetPasswordCommand,
            ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;

        public VerifyAccountAndSetPasswordCommandHandler(IUserRepository userRepository, IClock clock)
        {
            this._userRepository = userRepository;
            this._clock = clock;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            VerifyAccountAndSetPasswordCommand request, CancellationToken cancellationToken)
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
            VerifyAccountAndSetPasswordCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var convertedToken = new Guid(Convert.FromBase64String(request.Token));
            var userResult =
                await this._userRepository.FindByUserBySecurityToken(convertedToken, whenHappened, cancellationToken);
            if (userResult.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userResult.Value;

            if (user.IsVerified)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserIsAlreadyVerified));
            }

            user.VerifyAccount(whenHappened);

            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.NewPassword), whenHappened);

            user.CompleteTokenLifecycle(convertedToken, whenHappened);
            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
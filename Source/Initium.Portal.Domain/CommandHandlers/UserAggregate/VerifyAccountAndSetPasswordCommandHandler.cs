// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class
        VerifyAccountAndSetPasswordCommandHandler : IRequestHandler<VerifyAccountAndSetPasswordCommand,
            ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public VerifyAccountAndSetPasswordCommandHandler(IUserRepository userRepository, IClock clock,
            ILogger<VerifyAccountAndSetPasswordCommandHandler> logger)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            VerifyAccountAndSetPasswordCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(
            VerifyAccountAndSetPasswordCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var convertedToken = new Guid(Convert.FromBase64String(request.Token));
            var userResult =
                await this._userRepository.FindByUserBySecurityToken(convertedToken, whenHappened, cancellationToken);
            if (userResult.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userResult.Value;

            if (user.IsVerified)
            {
                this._logger.LogDebug("User already verified.");
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
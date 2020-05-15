// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class MarkAllUnreadNotificationsAsViewedCommandHandler
        : IRequestHandler<MarkAllUnreadNotificationsAsViewedCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;

        public MarkAllUnreadNotificationsAsViewedCommandHandler(IUserRepository userRepository, IClock clock)
        {
            this._userRepository = userRepository ??
                                   throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task<ResultWithError<ErrorData>> Handle(MarkAllUnreadNotificationsAsViewedCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(MarkAllUnreadNotificationsAsViewedCommand request, CancellationToken cancellationToken)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var userMaybe = await this._userRepository.Find(request.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            foreach (var notification in user.UserNotifications)
            {
                notification.MarkAsViewed(whenHappened);
            }

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
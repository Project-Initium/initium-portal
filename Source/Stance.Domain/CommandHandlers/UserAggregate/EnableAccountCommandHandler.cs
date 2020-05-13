// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class EnableAccountCommandHandler : IRequestHandler<EnableAccountCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;

        public EnableAccountCommandHandler(IUserRepository userRepository)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ResultWithError<ErrorData>> Handle(EnableAccountCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(EnableAccountCommand request, CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.Find(request.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            if (!user.IsDisabled)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotDisabled));
            }

            user.EnableAccount();

            user.AddDomainEvent(new UserEnabledEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName));

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
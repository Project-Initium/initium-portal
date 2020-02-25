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
using Stance.Queries.Contracts;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public sealed class
        CreateInitialUserCommandHandler : IRequestHandler<CreateInitialUserCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserQueries _userQueries;
        private readonly IUserRepository _userRepository;

        public CreateInitialUserCommandHandler(IUserRepository userRepository, IClock clock, IUserQueries userQueries)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            CreateInitialUserCommand request, CancellationToken cancellationToken)
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
            CreateInitialUserCommand request, CancellationToken cancellationToken)
        {
            var statusCheck = await this._userQueries.CheckForPresenceOfAnyUser(cancellationToken);

            if (statusCheck.IsPresent)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.SystemIsAlreadySetup));
            }

            var user = new User(Guid.NewGuid(), request.EmailAddress, BCrypt.Net.BCrypt.HashPassword(request.Password),
                true, this._clock.GetCurrentInstant().ToDateTimeUtc(),
                request.FirstName, request.LastName);

            this._userRepository.Add(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
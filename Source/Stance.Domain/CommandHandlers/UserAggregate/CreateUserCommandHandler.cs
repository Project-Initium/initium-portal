// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserQueries _userQueries;
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository, IClock clock, IUserQueries userQueries)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public async Task<Result<CreateUserCommandResult, ErrorData>> Handle(
            CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private static string GenerateRandomPassword()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[256];
                rng.GetBytes(data);
                return BCrypt.Net.BCrypt.HashPassword(Convert.ToBase64String(data));
            }
        }

        private async Task<Result<CreateUserCommandResult, ErrorData>> Process(
            CreateUserCommand request, CancellationToken cancellationToken)
        {
            var statusCheck =
                await this._userQueries.CheckForPresenceOfUserByEmailAddress(request.EmailAddress, cancellationToken);
            if (statusCheck.IsPresent)
            {
                return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserAlreadyExists));
            }

            var user = new User(Guid.NewGuid(), request.EmailAddress, GenerateRandomPassword(), request.IsLockable,
                this._clock.GetCurrentInstant().ToDateTimeUtc(), request.FirstName, request.LastName, request.Roles, request.IsAdmin);
            this._userRepository.Add(user);
            return Result.Ok<CreateUserCommandResult, ErrorData>(new CreateUserCommandResult(user.Id));
        }
    }
}
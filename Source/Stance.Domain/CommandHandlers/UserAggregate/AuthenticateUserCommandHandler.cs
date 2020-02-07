// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthenticateUserCommandResult, ErrorData>>
    {
        private readonly IUserRepository _userRepository;

        public AuthenticateUserCommandHandler(IUserRepository userRepository)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Result<AuthenticateUserCommandResult, ErrorData>> Handle(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<AuthenticateUserCommandResult, ErrorData>> Process(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var maybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (maybe.HasNoValue)
            {
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = maybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.AuthenticationFailed));
            }

            return Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(user.Id, user.EmailAddress));
        }
    }
}
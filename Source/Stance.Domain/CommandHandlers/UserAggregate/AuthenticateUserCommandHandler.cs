// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;

        public AuthenticateUserCommandHandler(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            AuthenticateUserCommand request, CancellationToken cancellationToken)
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
            AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var maybe = await this._userRepository.FindByEmailAddress(request.EmailAddress, cancellationToken);
            if (maybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = maybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed));
            }

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
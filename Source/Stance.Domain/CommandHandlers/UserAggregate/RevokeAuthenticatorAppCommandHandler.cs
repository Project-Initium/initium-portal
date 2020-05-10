// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public sealed class RevokeAuthenticatorAppCommandHandler :
        IRequestHandler<RevokeAuthenticatorAppCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;

        public RevokeAuthenticatorAppCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentUserService)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock;
            this._currentAuthenticatedUserProvider = currentUserService;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            RevokeAuthenticatorAppCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(RevokeAuthenticatorAppCommand request, CancellationToken cancellationToken)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUser.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.PasswordNotCorrect));
            }

            if (user.AuthenticatorApps.All(x => x.WhenRevoked != null))
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.NoAuthenticatorAppEnrolled));
            }

            user.RevokeAuthenticatorApp(this._clock.GetCurrentInstant().ToDateTimeUtc());

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
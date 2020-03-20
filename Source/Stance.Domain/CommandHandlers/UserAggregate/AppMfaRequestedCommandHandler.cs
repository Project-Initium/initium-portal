// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using ResultMonad;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public class AppMfaRequestedCommandHandler : IRequestHandler<AppMfaRequestedCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;

        public AppMfaRequestedCommandHandler(
            IUserRepository userRepository, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider,
            IClock clock)
        {
            this._userRepository = userRepository;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._clock = clock;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            AppMfaRequestedCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<ResultWithError<ErrorData>> Process(CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUserMaybe.Value.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                this._clock.GetCurrentInstant().ToDateTimeUtc(),
                AuthenticationHistoryType.AppMfaRequested);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
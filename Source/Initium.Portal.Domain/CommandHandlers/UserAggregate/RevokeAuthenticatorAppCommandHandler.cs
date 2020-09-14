// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public sealed class RevokeAuthenticatorAppCommandHandler :
        IRequestHandler<RevokeAuthenticatorAppCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public RevokeAuthenticatorAppCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentUserService, ILogger<RevokeAuthenticatorAppCommandHandler> logger)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._currentAuthenticatedUserProvider = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            RevokeAuthenticatorAppCommand request, CancellationToken cancellationToken)
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

        private async Task<ResultWithError<ErrorData>> Process(RevokeAuthenticatorAppCommand request, CancellationToken cancellationToken)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                this._logger.LogDebug("No active user.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUser.Value.UserId, cancellationToken);
            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                this._logger.LogDebug("Password is not valid.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.PasswordNotCorrect));
            }

            if (user.AuthenticatorApps.All(x => x.WhenRevoked != null))
            {
                this._logger.LogDebug("No auth app exists.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.NoAuthenticatorAppEnrolled));
            }

            user.RevokeAuthenticatorApp(this._clock.GetCurrentInstant().ToDateTimeUtc());

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
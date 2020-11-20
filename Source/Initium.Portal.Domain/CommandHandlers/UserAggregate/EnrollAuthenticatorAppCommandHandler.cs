// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public sealed class
        EnrollAuthenticatorAppCommandHandler : IRequestHandler<EnrollAuthenticatorAppCommand, ResultWithError<ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly ILogger _logger;

        public EnrollAuthenticatorAppCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, ILogger<EnrollAuthenticatorAppCommandHandler> logger)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            EnrollAuthenticatorAppCommand request, CancellationToken cancellationToken)
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
            EnrollAuthenticatorAppCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (!currentUserMaybe.HasValue || !(currentUserMaybe.Value is AuthenticatedUser currentUser))
            {
                this._logger.LogDebug("No active user.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var userMaybe = await this._userRepository.Find(currentUser.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            if (user.AuthenticatorApps.Any(x => x.WhenRevoked == null))
            {
                this._logger.LogDebug("Auth app exists.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticatorAppAlreadyEnrolled));
            }

            var secretBytes = Base32Encoding.ToBytes(request.Key);
            var topt = new Totp(secretBytes);
            var isVerified = topt.VerifyTotp(request.Code, out _);
            if (!isVerified)
            {
                this._logger.LogDebug("Auth app not verified.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.FailedVerifyingAuthenticatorCode));
            }

            user.EnrollAuthenticatorApp(Guid.NewGuid(), request.Key, this._clock.GetCurrentInstant().ToDateTimeUtc());

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
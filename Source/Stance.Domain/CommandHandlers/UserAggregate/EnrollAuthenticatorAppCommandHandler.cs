// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NodaTime;
using OtpNet;
using ResultMonad;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
{
    public sealed class
        EnrollAuthenticatorAppCommandHandler : IRequestHandler<EnrollAuthenticatorAppCommand, ResultWithError<ErrorData>
        >
    {
        private readonly IClock _clock;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public EnrollAuthenticatorAppCommandHandler(IUserRepository userRepository, IClock clock,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            EnrollAuthenticatorAppCommand request, CancellationToken cancellationToken)
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
            EnrollAuthenticatorAppCommand request, CancellationToken cancellationToken)
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasValue && currentUserMaybe.Value is AuthenticatedUser currentUser)
            {
                var userMaybe = await this._userRepository.Find(currentUser.UserId, cancellationToken);

                if (userMaybe.HasNoValue)
                {
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
                }

                var user = userMaybe.Value;

                if (user.AuthenticatorApps.Any(x => x.WhenRevoked == null))
                {
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticatorAppAlreadyEnrolled));
                }

                var secretBytes = Base32Encoding.ToBytes(request.Key);
                var topt = new Totp(secretBytes);
                var isVerified = topt.VerifyTotp(request.Code, out _);
                if (!isVerified)
                {
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.FailedVerifyingAuthenticatorCode));
                }

                user.EnrollAuthenticatorApp(Guid.NewGuid(), request.Key, this._clock.GetCurrentInstant().ToDateTimeUtc());

                return ResultWithError.Ok<ErrorData>();
            }

            return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
        }
    }
}
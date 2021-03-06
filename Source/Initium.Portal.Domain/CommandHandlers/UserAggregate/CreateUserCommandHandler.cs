﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserCommandResult, ErrorData>>
    {
        private readonly IClock _clock;
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository, IClock clock,
            IOptions<SecuritySettings> securitySettings, ILogger<CreateUserCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._clock = clock;
            this._logger = logger;
            this._securitySettings = securitySettings.Value;
        }

        public async Task<Result<CreateUserCommandResult, ErrorData>> Handle(
            CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = this.Process(request);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            if (dbResult.Error is UniquePersistenceError)
            {
                this._logger.LogDebug("Failed presence check");
                return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.UserAlreadyExists));
            }

            this._logger.LogDebug("Failed saving changes");
            return Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
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

        private Result<CreateUserCommandResult, ErrorData> Process(
            CreateUserCommand request)
        {
            var whenHappened = this._clock.GetCurrentInstant().ToDateTimeUtc();
            var user = new User(Guid.NewGuid(), request.EmailAddress, GenerateRandomPassword(), request.IsLockable,
                whenHappened, request.FirstName, request.LastName, request.Roles, request.IsAdmin);
            var token = user.GenerateNewAccountConfirmationToken(whenHappened, TimeSpan.FromMinutes(this._securitySettings.AccountVerificationTokenLifetime));
            user.AddIntegrationEvent(new AccountConfirmationTokenGeneratedIntegrationEvent(user.EmailAddress, user.Profile.FirstName, user.Profile.LastName, token.Id, token.WhenExpires));
            this._userRepository.Add(user);
            return Result.Ok<CreateUserCommandResult, ErrorData>(new CreateUserCommandResult(user.Id));
        }
    }
}
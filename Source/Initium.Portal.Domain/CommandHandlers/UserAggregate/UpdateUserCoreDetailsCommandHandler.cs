// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class UpdateUserCoreDetailsCommandHandler : IRequestHandler<UpdateUserCoreDetailsCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCoreDetailsCommandHandler> _logger;

        public UpdateUserCoreDetailsCommandHandler(IUserRepository userRepository, ILogger<UpdateUserCoreDetailsCommandHandler> logger)
        {
            this._userRepository = userRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            UpdateUserCoreDetailsCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            if (dbResult.Error is UniquePersistenceError)
            {
                this._logger.LogDebug("Failed presence check.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserAlreadyExists));
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(
            UpdateUserCoreDetailsCommand request, CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.Find(request.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;

            user.UpdateSystemAccessDetails(request.EmailAddress, request.IsLockable);
            user.UpdateProfile(request.FirstName, request.LastName);
            user.SetAdminStatus(request.IsAdmin);
            user.SetRoles(request.Roles);

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
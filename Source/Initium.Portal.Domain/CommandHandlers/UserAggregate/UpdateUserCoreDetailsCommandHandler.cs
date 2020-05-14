// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts.Static;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class UpdateUserCoreDetailsCommandHandler : IRequestHandler<UpdateUserCoreDetailsCommand, ResultWithError<ErrorData>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserQueries _userQueries;

        public UpdateUserCoreDetailsCommandHandler(IUserRepository userRepository, IUserQueries userQueries)
        {
            this._userRepository = userRepository;
            this._userQueries = userQueries;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            UpdateUserCoreDetailsCommand request, CancellationToken cancellationToken)
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
            UpdateUserCoreDetailsCommand request, CancellationToken cancellationToken)
        {
            var userMaybe = await this._userRepository.Find(request.UserId, cancellationToken);

            if (userMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.UserNotFound));
            }

            var user = userMaybe.Value;
            if (!string.Equals(request.EmailAddress, user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var statusCheck =
                    await this._userQueries.CheckForPresenceOfUserByEmailAddress(request.EmailAddress);
                if (statusCheck.IsPresent)
                {
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.UserAlreadyExists));
                }
            }

            user.UpdateSystemAccessDetails(request.EmailAddress, request.IsLockable);
            user.UpdateProfile(request.FirstName, request.LastName);
            user.SetAdminStatus(request.IsAdmin);
            user.SetRoles(request.Roles);

            this._userRepository.Update(user);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
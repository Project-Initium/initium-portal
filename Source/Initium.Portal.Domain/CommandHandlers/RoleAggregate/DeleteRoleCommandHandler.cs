// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts.Static;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.RoleAggregate
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ResultWithError<ErrorData>>
    {
        private readonly IRoleQueries _roleQueries;
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository, IRoleQueries roleQueries)
        {
            this._roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this._roleQueries = roleQueries ?? throw new ArgumentNullException(nameof(roleQueries));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        public async Task<ResultWithError<ErrorData>> Process(
            DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleMaybe = await this._roleRepository.Find(request.RoleId, cancellationToken);
            if (roleMaybe.HasNoValue)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleNotFound));
            }

            var presenceResult = await this._roleQueries.CheckForRoleUsageById(request.RoleId);
            if (presenceResult.IsPresent)
            {
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleInUse));
            }

            var role = roleMaybe.Value;

            this._roleRepository.Delete(role);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
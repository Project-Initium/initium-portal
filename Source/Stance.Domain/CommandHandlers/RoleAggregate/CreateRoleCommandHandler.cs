// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Stance.Domain.CommandResults.RoleAggregate;
using Stance.Domain.Commands.RoleAggregate;
using Stance.Queries.Contracts;

namespace Stance.Domain.CommandHandlers.RoleAggregate
{
    public class
        CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<CreateRoleCommandResult, ErrorData>>
    {
        private readonly IRoleQueries _roleQueries;
        private readonly IRoleRepository _roleRepository;

        public CreateRoleCommandHandler(IRoleRepository roleRepository, IRoleQueries roleQueries)
        {
            this._roleRepository = roleRepository;
            this._roleQueries = roleQueries;
        }

        public async Task<Result<CreateRoleCommandResult, ErrorData>> Handle(
            CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<CreateRoleCommandResult, ErrorData>> Process(
            CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var presenceResult = await this._roleQueries.CheckForPresenceOfRoleByName(request.Name, cancellationToken);
            if (presenceResult.IsPresent)
            {
                return Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(ErrorCodes.RoleAlreadyExists));
            }

            var role = new Role(Guid.NewGuid(), request.Name, request.Resources);
            this._roleRepository.Add(role);

            return Result.Ok<CreateRoleCommandResult, ErrorData>(new CreateRoleCommandResult(role.Id));
        }
    }
}
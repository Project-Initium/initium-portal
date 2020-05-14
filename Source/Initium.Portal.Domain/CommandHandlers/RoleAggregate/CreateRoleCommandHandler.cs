// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.CommandResults.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts.Static;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.RoleAggregate
{
    public class
        CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<CreateRoleCommandResult, ErrorData>>
    {
        private readonly IRoleQueries _roleQueries;
        private readonly IRoleRepository _roleRepository;

        public CreateRoleCommandHandler(IRoleRepository roleRepository, IRoleQueries roleQueries)
        {
            this._roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this._roleQueries = roleQueries ?? throw new ArgumentNullException(nameof(roleQueries));
        }

        public async Task<Result<CreateRoleCommandResult, ErrorData>> Handle(
            CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<Result<CreateRoleCommandResult, ErrorData>> Process(
            CreateRoleCommand request)
        {
            var presenceResult = await this._roleQueries.CheckForPresenceOfRoleByName(request.Name);
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
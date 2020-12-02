// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.CommandResults.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.RoleAggregate
{
    public class CreateRoleCommandHandler
        : IRequestHandler<CreateRoleCommand, Result<CreateRoleCommandResult, ErrorData>>
    {
        private readonly IRoleQueryService _roleQueryService;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(IRoleRepository roleRepository, IRoleQueryService roleQueryService, ILogger<CreateRoleCommandHandler> logger)
        {
            this._roleRepository = roleRepository;
            this._roleQueryService = roleQueryService;
            this._logger = logger;
        }

        public async Task<Result<CreateRoleCommandResult, ErrorData>> Handle(
            CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<Result<CreateRoleCommandResult, ErrorData>> Process(
            CreateRoleCommand request)
        {
            var presenceResult = await this._roleQueryService.CheckForPresenceOfRoleByName(request.Name);
            if (presenceResult.IsPresent)
            {
                this._logger.LogDebug("Failed presence check.");
                return Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(ErrorCodes.RoleAlreadyExists));
            }

            var role = new Role(Guid.NewGuid(), request.Name, request.Resources);
            this._roleRepository.Add(role);

            return Result.Ok<CreateRoleCommandResult, ErrorData>(new CreateRoleCommandResult(role.Id));
        }
    }
}
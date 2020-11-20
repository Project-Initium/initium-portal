// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.RoleAggregate
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ResultWithError<ErrorData>>
    {
        private readonly IRoleQueryService _roleQueryService;
        private readonly IResourceQueryService _resourceQueryService;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger _logger;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository, IRoleQueryService roleQueryService, ILogger<UpdateRoleCommandHandler> logger, IResourceQueryService resourceQueryService)
        {
            this._roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this._roleQueryService = roleQueryService ?? throw new ArgumentNullException(nameof(roleQueryService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._resourceQueryService = resourceQueryService ?? throw new ArgumentNullException(nameof(resourceQueryService));
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                this._logger.LogDebug("Failed saving changes.");
                return ResultWithError.Fail(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
        }

        private async Task<ResultWithError<ErrorData>> Process(
            UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleMaybe = await this._roleRepository.Find(request.RoleId, cancellationToken);
            if (roleMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleNotFound));
            }

            var role = roleMaybe.Value;

            if (!string.Equals(role.Name, request.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var presenceResult =
                    await this._roleQueryService.CheckForPresenceOfRoleByName(request.Name);
                if (presenceResult.IsPresent)
                {
                    this._logger.LogDebug("Failed presence check.");
                    return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleAlreadyExists));
                }
            }

            var systemResources = await this._resourceQueryService.GetFeatureStatusBasedResources(cancellationToken);

            var resources = request.Resources.ToList();

            resources.AddRange(from roleResource in role.RoleResources where systemResources.Any(x => !x.IsEnabled && x.Id == roleResource.Id) select roleResource.Id);

            role.UpdateName(request.Name);
            role.SetResources(resources);

            this._roleRepository.Update(role);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
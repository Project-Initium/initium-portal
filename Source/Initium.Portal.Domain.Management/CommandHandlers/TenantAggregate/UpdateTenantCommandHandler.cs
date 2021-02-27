// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Queries.Management.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Common.Domain.CommandHandlers.TenantAggregate
{
    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ResultWithError<ErrorData>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;

        public UpdateTenantCommandHandler(ITenantRepository tenantRepository, ILogger<UpdateTenantCommandHandler> logger)
        {
            this._tenantRepository = tenantRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._tenantRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            if (dbResult.Error is UniquePersistenceError)
            {
                this._logger.LogDebug("Failed presence check.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.TenantAlreadyExists));
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantMaybe = await this._tenantRepository.Find(request.TenantId, cancellationToken);
            if (tenantMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.TenantNotFound));
            }

            var tenant = tenantMaybe.Value;

            tenant.UpdateDetails(request.Identifier, tenant.Name, tenant.ConnectionString);

            tenant.SetSystemFeatures(request.SystemFeatures);

            this._tenantRepository.Update(tenant);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
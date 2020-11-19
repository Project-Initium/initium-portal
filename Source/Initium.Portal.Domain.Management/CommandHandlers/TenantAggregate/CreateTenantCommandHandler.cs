using Microsoft.Extensions.Logging;
﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Domain;
using Initium.Portal.Queries.Management.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Common.Domain.CommandHandlers.TenantAggregate
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ResultWithError<ErrorData>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;
        private readonly ITenantQueryService _tenantQueryService;

        public CreateTenantCommandHandler(ITenantRepository tenantRepository, ILogger<CreateTenantCommandHandler> logger, ITenantQueryService tenantQueryService)
        {
            this._tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
        }

        public async Task<ResultWithError<ErrorData>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request);
            var dbResult = await this._tenantRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(CreateTenantCommand request)
        {
            var presenceResult = await this._tenantQueryService.CheckForPresenceOfTenantByIdentifier(request.Identifier);
            if (presenceResult.IsPresent)
            {
                this._logger.LogDebug("Failed presence check.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.TenantAlreadyExists));
            }

            var tenant = new Tenant(request.TenantId, request.Identifier, request.Name, request.ConnectionString);
            tenant.SetSystemFeatures(request.SystemFeatures);
            this._tenantRepository.Add(tenant);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
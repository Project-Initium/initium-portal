// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

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
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ResultWithError<ErrorData>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;

        public CreateTenantCommandHandler(ITenantRepository tenantRepository, ILogger<CreateTenantCommandHandler> logger)
        {
            this._tenantRepository = tenantRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var result = this.Process(request);
            var dbResult = await this._tenantRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            if (dbResult.Error is UniquePersistenceError)
            {
                this._logger.LogDebug("Failed presence check");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.TenantAlreadyExists));
            }

            this._logger.LogDebug("Failed saving changes");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private ResultWithError<ErrorData> Process(CreateTenantCommand request)
        {
            var tenant = new Tenant(request.TenantId, request.Identifier, request.Name, request.ConnectionString);
            tenant.SetSystemFeatures(request.SystemFeatures);
            this._tenantRepository.Add(tenant);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Domain.Commands.TenantAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.TenantAggregate
{
    public class DisableTenantCommandHandler : IRequestHandler<DisableTenantCommand, ResultWithError<ErrorData>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;
        private readonly IClock _clock;

        public DisableTenantCommandHandler(ITenantRepository tenantRepository, ILogger<EnableTenantCommandHandler> logger, IClock clock)
        {
            this._tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task<ResultWithError<ErrorData>> Handle(DisableTenantCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._tenantRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(DisableTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantMaybe = await this._tenantRepository.Find(request.TenantId, cancellationToken);
            if (tenantMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleNotFound));
            }

            var tenant = tenantMaybe.Value;

            tenant.Disable(this._clock.GetCurrentInstant().ToDateTimeUtc());

            this._tenantRepository.Update(tenant);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}
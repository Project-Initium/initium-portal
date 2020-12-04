// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Common.Domain.Commands.TenantAggregate
{
    public class DisableTenantCommand : IRequest<ResultWithError<ErrorData>>
    {
        public DisableTenantCommand(Guid tenantId)
        {
            this.TenantId = tenantId;
        }

        public Guid TenantId { get; }
    }
}
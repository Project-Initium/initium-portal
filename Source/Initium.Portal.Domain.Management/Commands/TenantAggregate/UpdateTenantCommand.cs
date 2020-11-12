// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Common.Domain.Commands.TenantAggregate
{
    public class UpdateTenantCommand : IRequest<ResultWithError<ErrorData>>
    {
        public UpdateTenantCommand(Guid tenantId, string identifier, string name)
        {
            this.TenantId = tenantId;
            this.Identifier = identifier;
            this.Name = name;
        }

        public Guid TenantId { get; }

        public string Identifier { get; }

        public string Name { get; }
    }
}
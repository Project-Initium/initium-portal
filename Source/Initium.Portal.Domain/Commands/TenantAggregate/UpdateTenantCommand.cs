﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.TenantAggregate
{
    public class UpdateTenantCommand : IRequest<ResultWithError<ErrorData>>
    {
        public UpdateTenantCommand(Guid tenantId, string identifier, string name, IReadOnlyList<Guid> features)
        {
            this.TenantId = tenantId;
            this.Identifier = identifier;
            this.Name = name;
            this.Features = features;
        }

        public Guid TenantId { get; }

        public string Identifier { get; }

        public string Name { get; }

        public IReadOnlyList<Guid> Features { get; }
    }
}
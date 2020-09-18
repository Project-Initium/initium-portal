// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.TenantAggregate
{
    public class CreateTenantCommand : IRequest<ResultWithError<ErrorData>>
    {
        public CreateTenantCommand(Guid id, string identifier, string name, string connectionString)
        {
            this.Id = id;
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
        }

        public Guid Id { get; }

        public string Identifier { get; }

        public string Name { get; }

        public string ConnectionString { get; }
    }
}
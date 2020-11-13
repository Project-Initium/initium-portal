// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Queries;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models.Resource;
using MaybeMonad;

namespace Initium.Portal.Queries.Contracts
{
    public interface IResourceQueryService : IQueryService<ResourceReadEntity>
    {
        Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources();
    }
}
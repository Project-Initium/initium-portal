// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Initium.Portal.Core.Contracts.Queries
{
    public interface IQueryService<out TReadEntity>
        where TReadEntity : ReadOnlyEntity
    {
        IQueryable<TReadEntity> QueryableEntity { get; }
    }
}
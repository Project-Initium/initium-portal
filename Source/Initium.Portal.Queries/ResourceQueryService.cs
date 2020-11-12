// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models.Resource;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public class ResourceQueryService : IResourceQueryService
    {
        private readonly ICoreQueryContext _context;

        public ResourceQueryService(ICoreQueryContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<Resource> QueryableEntity => this._context.Resources;

        public async Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources()
        {
            var data = await this._context.Resources.ToListAsync();
            if (data == null || data.Count == 0)
            {
                return Maybe<List<SimpleResourceModel>>.Nothing;
            }

            var items = data.Select(
                i => new SimpleResourceModel(i.Id, i.Name, i.ParentResourceId)).ToList();

            foreach (var i in items)
            {
                i.SetSimpleResources(items.Where(n => n.ParentId == i.Id).ToList());
            }

            return Maybe.From(new List<SimpleResourceModel>(items.Where(n => n.ParentId == null)));
        }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

namespace Initium.Portal.Queries
{
    public class ResourceQueryService : IResourceQueryService
    {
        private readonly ICoreQueryContext _context;
        private readonly IFeatureManager _featureManager;

        public ResourceQueryService(ICoreQueryContext context, IFeatureManager featureManager)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        public IQueryable<ResourceReadEntity> QueryableEntity => this._context.Resources;

        public async Task<IReadOnlyList<SimpleResourceModel>> GetFeatureBasedNestedSimpleResources(CancellationToken cancellationToken = default)
        {
            var data = await this._context.Resources.ToListAsync(cancellationToken: cancellationToken);
            if (data == null || data.Count == 0)
            {
                return new List<SimpleResourceModel>();
            }

            var filteredPermissions = new List<SimpleResourceModel>();
            foreach (var resourceReadEntity in data)
            {
                if (
                    string.IsNullOrWhiteSpace(resourceReadEntity.FeatureCode)
                    || await this._featureManager.IsEnabledAsync(resourceReadEntity.FeatureCode))
                {
                    filteredPermissions.Add(new SimpleResourceModel(
                        resourceReadEntity.Id,
                        resourceReadEntity.Name,
                        resourceReadEntity.ParentResourceId,
                        resourceReadEntity.FeatureCode));
                }
            }

            foreach (var i in filteredPermissions)
            {
                i.SetSimpleResources(filteredPermissions.Where(n => n.ParentId == i.Id).ToList());
            }

            return new List<SimpleResourceModel>(filteredPermissions.Where(n => n.ParentId == null));
        }

        public async Task<IReadOnlyList<FeatureStatusBasedResource>> GetFeatureStatusBasedResources(CancellationToken cancellationToken = default)
        {
            var data = await this._context.Resources
                .Select(x => new
                {
                    x.Id,
                    x.FeatureCode,
                }).ToListAsync(cancellationToken: cancellationToken);

            if (data == null || data.Count == 0)
            {
                return new List<FeatureStatusBasedResource>();
            }

            var filteredPermissions = new List<FeatureStatusBasedResource>();
            foreach (var resourceReadEntity in data)
            {
                filteredPermissions.Add(new FeatureStatusBasedResource(
                    resourceReadEntity.Id,
                    string.IsNullOrWhiteSpace(resourceReadEntity.FeatureCode)
                    || await this._featureManager.IsEnabledAsync(resourceReadEntity.FeatureCode)));
            }

            return filteredPermissions;
        }
    }
}
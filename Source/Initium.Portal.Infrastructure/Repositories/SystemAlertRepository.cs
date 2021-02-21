// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Repositories
{
    public class SystemAlertRepository : ISystemAlertRepository
    {
        private readonly GenericDataContext _dataContext;

        public SystemAlertRepository(GenericDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public IUnitOfWork UnitOfWork => this._dataContext;

        public ISystemAlert Add(ISystemAlert systemAlert)
        {
            var entity = systemAlert as SystemAlert;
            if (entity == null)
            {
                throw new ArgumentException(nameof(systemAlert));
            }

            return this._dataContext.Set<SystemAlert>().Add(entity).Entity;
        }

        public async Task<Maybe<ISystemAlert>> Find(Guid id, CancellationToken cancellationToken = default)
        {
            var systemAlert = await this._dataContext.Set<SystemAlert>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            await this.Refresh(systemAlert);
            return Maybe.From<ISystemAlert>(systemAlert);
        }

        public void Delete(ISystemAlert systemAlert)
        {
            var entity = systemAlert as SystemAlert;
            if (entity == null)
            {
                throw new ArgumentException(nameof(systemAlert));
            }

            this._dataContext.Set<SystemAlert>().Remove(entity);
        }

        public void Update(ISystemAlert systemAlert)
        {
            var entity = systemAlert as SystemAlert;
            if (entity == null)
            {
                throw new ArgumentException(nameof(systemAlert));
            }

            this._dataContext.Set<SystemAlert>().Update(entity);
        }

        private async Task Refresh(SystemAlert systemAlert)
        {
            if (systemAlert != null)
            {
                await this._dataContext.Entry(systemAlert).ReloadAsync();
            }
        }
    }
}
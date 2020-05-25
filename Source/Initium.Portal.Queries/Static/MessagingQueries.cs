// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Dynamic;
using Initium.Portal.Queries.Static.Models.Messaging;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.Static
{
    public class MessagingQueries : IMessagingQueries
    {
        private readonly ODataContext _context;

        public MessagingQueries(ODataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Maybe<IReadOnlyList<ActiveSystemAlert>>> GetActiveSystemAlerts()
        {
            var data = await this._context.SystemAlerts.Where(x => x.IsActive)
                .Select(x => new ActiveSystemAlert(x.Message, x.Type)).ToListAsync();

            return data == null || !data.Any()
                ? Maybe<IReadOnlyList<ActiveSystemAlert>>.Nothing
                : Maybe.From<IReadOnlyList<ActiveSystemAlert>>(data);
        }

        public async Task<Maybe<DetailedSystemAlert>> GetDetailedSystemAlertById(Guid id)
        {
            var data = await this._context.SystemAlerts
                .SingleOrDefaultAsync(x => x.Id == id);

            return data == null ? Maybe<DetailedSystemAlert>.Nothing : Maybe.From(new DetailedSystemAlert(data.Id, data.Name, data.Message, data.Type, data.WhenToShow, data.WhenToHide));
        }
    }
}
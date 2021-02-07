// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models.Notifications;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public class UserNotificationQueryService : IUserNotificationQueryService
    {
        private readonly ICoreQueryContext _context;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserNotificationQueryService(ICoreQueryContext context, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        public IQueryable<UserNotificationReadEntity> QueryableEntity
        {
            get
            {
                var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
                return currentUser.HasNoValue ? new List<UserNotificationReadEntity>().AsQueryable() : this._context.UserNotifications.Where(x => x.UserId == currentUser.Value.UserId);
            }
        }

        public async Task<Maybe<List<SimpleNotification>>> GetLatestNotifications(int top)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return Maybe<List<SimpleNotification>>.Nothing;
            }

            return Maybe.From(await this._context.UserNotifications
                .Where(x => x.UserId == currentUser.Value.UserId)
                .OrderBy(x => x.WhenNotified).Take(top)
                .Select(x => new SimpleNotification(
                    x.Subject,
                    x.Message,
                    x.Type,
                    x.SerializedEventData,
                    x.WhenViewed.HasValue)).ToListAsync());
        }

        public async Task<bool> AnyUnread()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return false;
            }

            return await this._context.UserNotifications
                .Where(x => x.UserId == currentUser.Value.UserId)
                .AnyAsync(x => x.WhenViewed == null);
        }
    }
}
// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Stance.Queries.Dynamic;
using Stance.Queries.Dynamic.Entities;

namespace Stance.Web.Controllers.OData
{
    [ODataRoutePrefix("User")]
    public class UserController : ODataController
    {
        private readonly ODataContext _context;

        public UserController(ODataContext context)
        {
            this._context = context;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        [ODataRoute("")]
        public IQueryable<User> Get()
        {
            return this._context.Users;
        }

        //[Authorize]
        [ODataRoute("search")]
        [HttpPost]
        public IQueryable<User> Search([FromBody] SearchModel model)
        {
            if (model == null)
            {
                return this._context.Users;
            }
            
            var predicate = PredicateBuilder.New<User>(true);
            if (model.IsLocked.HasValue)
            {
                predicate.Or(x => x.IsLocked == model.IsLocked.Value);
            }
            
            if (model.IsVerified.HasValue)
            {
                predicate.Or(x => x.IsVerified == model.IsVerified.Value);
            }
            
            if (model.IsAdmin.HasValue)
            {
                predicate.Or(x => x.IsAdmin == model.IsAdmin.Value);
            }

            if (model.LastAuthenticatedFrom.HasValue && model.LastAuthenticatedTo.HasValue)
            {
                predicate.Or(x =>
                    x.WhenLastAuthenticated > model.LastAuthenticatedFrom &&
                    x.WhenLastAuthenticated < model.LastAuthenticatedTo);
            }
            else if (model.LastAuthenticatedFrom.HasValue)
            {
                predicate.Or(x => x.WhenLastAuthenticated > model.LastAuthenticatedFrom);
            }
            else if (model.LastAuthenticatedTo.HasValue)
            {
                predicate.Or(x => x.WhenLastAuthenticated < model.LastAuthenticatedFrom);
            }

            if (model.Roles.Any())
            {
                var p = PredicateBuilder.New<User>(true);
                foreach (var role in model.Roles)
                {
                    p.And(x => x.UserRoles.Any(x => x.RoleId == role));
                }

                predicate.Or(p);
            }
            
           return this._context.Users.Where (predicate);
        }
    }

    public class SearchModel
    {
        public SearchModel()
        {
            this.Roles = new List<Guid>();
        }
        public bool? IsLocked { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? LastAuthenticatedFrom { get; set; }
        public DateTime? LastAuthenticatedTo { get; set; }
        public List<Guid> Roles { get; set; }
    }
}
// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Stance.Queries.OData;
using Stance.Queries.OData.Entities;

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
    }

    [ODataRoutePrefix("Role")]
    public class RoleController : ODataController
    {
        private readonly ODataContext _context;

        public RoleController(ODataContext context)
        {
            this._context = context;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        [ODataRoute("")]
        public IQueryable<Role> Get()
        {
            return this._context.Roles;
        }
    }
}
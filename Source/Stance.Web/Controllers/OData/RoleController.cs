// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Stance.Queries.Dynamic;
using Stance.Queries.Dynamic.Entities;

namespace Stance.Web.Controllers.OData
{
    [ODataRoutePrefix("Role")]
    public class RoleController : ODataController
    {
        private readonly ODataContext _context;

        public RoleController(ODataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        [ODataRoute("")]
        public IQueryable<Role> Get()
        {
            return this._context.Roles;
        }
    }
}
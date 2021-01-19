// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Initium.Portal.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Moq;

namespace Initium.Portal.Tests.Web.ODataEndpoints
{
    [SuppressMessage("Minor Code Smell", "S3881:URIs should not be hardcoded",
    Justification = "This is just for unit testing")]
    public abstract class BaseODataControllerTest<TEntity> : IDisposable
        where TEntity : ReadEntity
    {
        protected BaseODataControllerTest()
        {
            var collection = new ServiceCollection();

            collection.AddOData();
            //collection.AddODataQueryFilter();
            collection.AddTransient<ODataUriResolver>();
            collection.AddTransient<ODataQueryValidator>();
            collection.AddTransient<TopQueryValidator>();

            this.Provider = collection.BuildServiceProvider();

            var routeBuilder =
                new RouteBuilder(Mock.Of<IApplicationBuilder>(x => x.ApplicationServices == this.Provider));
            //routeBuilder.EnableDependencyInjection();
        }

        protected abstract IEdmModel EdmModel { get; }

        protected IServiceProvider Provider { get; private set; }

        public void Dispose()
        {
            this.Provider = null;
        }

        protected ODataQueryOptions<TEntity> GenerateEmptyQueryOptions()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = this.Provider,
            };

            var actionArguments = new Dictionary<string, object>();
            var actionContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), new ModelStateDictionary()),
                new List<IFilterMetadata>(), actionArguments, null);
            var context = new ODataQueryContext(this.EdmModel, typeof(TEntity), new ODataPath());
            return new ODataQueryOptions<TEntity>(context, actionContext.HttpContext.Request);
        }
    }
}
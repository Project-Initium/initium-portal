// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.Tenant;
using Initium.Portal.Web.Management.Pages.App.Tenants;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web_Management.Pages.App.Tenant
{
    public class ViewTenantTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoTenantFound_ExpectNotFoundResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<TenantMetadata>.Nothing);

            var page = new ViewTenant(tenantQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenTenantFound_ExpectPageResultAndDataSet()
        {
            var tenantMetadata = new TenantMetadata(
                TestVariables.TenantId,
                "identifier",
                "name",
                null);
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenantMetadata));

            var page = new ViewTenant(tenantQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(tenantMetadata, page.Tenant);
        }
    }
}
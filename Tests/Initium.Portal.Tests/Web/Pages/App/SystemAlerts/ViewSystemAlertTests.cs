// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Messaging;
using Initium.Portal.Web.Pages.App.SystemAlerts;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.SystemAlerts
{
    public class ViewSystemAlertTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoSystemAlertFound_ExpectNotFoundResult()
        {
            var messagingQueries = new Mock<ISystemAlertQueryService>();
            messagingQueries.Setup(x => x.GetDetailedSystemAlertById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe<DetailedSystemAlert>.Nothing);

            var page = new ViewSystemAlert(messagingQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenSystemAlertFound_ExpectPageResultAndDataSet()
        {
            var detailedSystemAlert = new DetailedSystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now,
                TestVariables.Now.AddHours(1));
            var messagingQueries = new Mock<ISystemAlertQueryService>();
            messagingQueries.Setup(x => x.GetDetailedSystemAlertById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(detailedSystemAlert));

            var page = new ViewSystemAlert(messagingQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(detailedSystemAlert, page.SystemAlert);
        }
    }
}
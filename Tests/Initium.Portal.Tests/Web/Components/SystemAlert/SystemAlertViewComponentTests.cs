// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Messaging;
using Initium.Portal.Web.Components.SystemAlert;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Components.SystemAlert
{
    public class SystemAlertViewComponentTests
    {
        [Fact]
        public async Task InvokeAsync_GivenNoData_ExpectResultWithEmptyList()
        {
            var messagingQueries = new Mock<ISystemAlertQueryService>();
            messagingQueries.Setup(x => x.GetActiveSystemAlerts())
                .ReturnsAsync(Maybe<IReadOnlyList<ActiveSystemAlert>>.Nothing);

            var component = new SystemAlertViewComponent(messagingQueries.Object);

            var result = await component.InvokeAsync();
            var viewComponentResult = Assert.IsType<ViewViewComponentResult>(result);
            var data = Assert.IsType<ViewDataDictionary<IReadOnlyList<ActiveSystemAlert>>>(viewComponentResult.ViewData);
            Assert.Empty(data);
        }

        [Fact]
        public async Task InvokeAsync_GivenActiveSystemAlerts_ExpectResultWithDataInList()
        {
            var activeSystemAlerts = new List<ActiveSystemAlert>
            {
                new ActiveSystemAlert("message", SystemAlertType.Critical),
                new ActiveSystemAlert("message", SystemAlertType.Critical),
            };
            var messagingQueries = new Mock<ISystemAlertQueryService>();
            messagingQueries.Setup(x => x.GetActiveSystemAlerts())
                .ReturnsAsync(Maybe.From<IReadOnlyList<ActiveSystemAlert>>(activeSystemAlerts.AsReadOnly()));

            var component = new SystemAlertViewComponent(messagingQueries.Object);

            var result = await component.InvokeAsync();
            var viewComponentResult = Assert.IsType<ViewViewComponentResult>(result);
            var data = Assert.IsType<ViewDataDictionary<IReadOnlyList<ActiveSystemAlert>>>(viewComponentResult.ViewData);
            Assert.Equal(2, data.Model.Count);
        }
    }
}
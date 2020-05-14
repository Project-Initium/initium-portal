// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Components.LeftNav;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;

namespace Initium.Portal.Tests.Web.Components.LeftNav
{
    public class LeftNavViewComponentTests
    {
        [Fact]
        public void Invoke_GivenNoArguments_ExpectView()
        {
            var leftNavViewComponent = new LeftNavViewComponent();
            var result = leftNavViewComponent.Invoke();
            Assert.IsType<ViewViewComponentResult>(result);
        }
    }
}
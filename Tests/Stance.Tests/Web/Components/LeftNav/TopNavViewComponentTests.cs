// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using Stance.Web.Components.TopNav;
using Xunit;

namespace Stance.Tests.Web.Components.LeftNav
{
    public class TopNavViewComponentTests
    {
        [Fact]
        public void Invoke_GivenNoArguments_ExpectView()
        {
            var topNavViewComponent = new TopNavViewComponent();
            var result = topNavViewComponent.Invoke();
            Assert.IsType<ViewViewComponentResult>(result);
        }
    }
}
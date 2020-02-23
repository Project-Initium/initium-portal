// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using Stance.Web.Components.LeftNav;
using Xunit;

namespace Stance.Tests.Web.Components.LeftNav
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
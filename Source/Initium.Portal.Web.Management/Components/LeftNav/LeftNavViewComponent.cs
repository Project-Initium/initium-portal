// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Components.LeftNav
{
    public class LeftNavViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}
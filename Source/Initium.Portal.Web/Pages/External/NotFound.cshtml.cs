// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Initium.Portal.Web.Pages.External
{
    public class NotFound : PageModel
    {
        public void OnGet()
        {
            this.Response.StatusCode = 200;
        }
    }
}
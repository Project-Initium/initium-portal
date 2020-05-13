// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Initium.Portal.Web.Pages.Auth
{
    public class SignOut : PageModel
    {
        public async Task OnGetAsync()
        {
            await this.HttpContext.SignOutAsync();
        }
    }
}
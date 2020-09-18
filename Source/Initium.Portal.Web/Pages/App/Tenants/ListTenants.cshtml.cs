// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Initium.Portal.Web.Pages.App.Tenants
{
    [SystemOwnerAuthorize]
    public class ListTenants : PageModel
    {
    }
}
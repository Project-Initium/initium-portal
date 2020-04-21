// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stance.Web.Infrastructure.Formatters;

namespace Stance.Web.Controllers.Api.Security
{
    public class SecurityApiController : Controller
    {
        private readonly ILogger _logger;

        public SecurityApiController(ILogger<SecurityApiController> logger)
        {
            this._logger = logger;
        }

        [HttpPost("api/security/csp-report")]
        public IActionResult Report([FromBody] CspPost report)
        {
            this._logger.LogCritical("CSP Violation: {Report}", report);
            return this.Ok();
        }
    }
}
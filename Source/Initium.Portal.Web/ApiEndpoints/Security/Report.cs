// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Ardalis.ApiEndpoints;
using Initium.Portal.Web.Infrastructure.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Web.ApiEndpoints.Security
{
    public class Report : BaseEndpoint<CspPost, BasicEndpointResponse>
    {
        private readonly ILogger _logger;

        public Report(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/security/csp-report")]
        public override ActionResult<BasicEndpointResponse> Handle(CspPost request)
        {
            this._logger.LogCritical("CSP Violation: {Report}", request);
            return this.Ok();
        }
    }
}
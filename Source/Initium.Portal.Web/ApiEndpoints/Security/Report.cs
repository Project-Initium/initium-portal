// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Ardalis.ApiEndpoints;
using Initium.Portal.Web.Infrastructure.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Web.ApiEndpoints.Security
{
    public class Report : BaseEndpoint
        .WithRequest<CspPost>
        .WithoutResponse
    {
        private readonly ILogger _logger;

        public Report(ILogger<Report> logger)
        {
            this._logger = logger;
        }

        [HttpPost("api/security/csp-report", Name = "ReportEndpoint")]
        public override ActionResult Handle(CspPost request)
        {
            this._logger.LogCritical("CSP Violation: {Report}", request);
            return this.Ok();
        }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure.Formatters
{
    public class CspReportBodyFormatter : InputFormatter
    {
        public CspReportBodyFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/csp-report"));
        }

        public override bool CanRead(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;
            return contentType == "application/csp-report";
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (contentType != "application/csp-report")
            {
                return await InputFormatterResult.FailureAsync();
            }

            using var reader = new StreamReader(request.Body);
            var content = await reader.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<CspPost>(content);
            return await InputFormatterResult.SuccessAsync(data);
        }
    }
}
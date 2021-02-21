// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Initium.Portal.Web.Infrastructure.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Xunit;

namespace Initium.Portal.Tests.Web.Infrastructure.Formatters
{
    public class CspReportBodyFormatterTests
    {
        [Fact]
        public void CanRead_GivenCorrectContentType_ExpectTrue()
        {
            var cspReportBodyFormatter = new CspReportBodyFormatter();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/csp-report";
            Assert.True(cspReportBodyFormatter.CanRead(CreateInputFormatterContext(typeof(CspPost), httpContext)));
        }

        [Fact]
        public void CanRead_GivenWrongContentTypeNull_ExpectFalse()
        {
            var cspReportBodyFormatter = new CspReportBodyFormatter();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = string.Empty;
            Assert.False(
                cspReportBodyFormatter.CanRead(CreateInputFormatterContext(typeof(CspPost), httpContext)));
        }

        [Fact]
        public async Task ReadRequestBodyAsync_GivenInvalidContentType_ExpectFailure()
        {
            var cspReportBodyFormatter = new CspReportBodyFormatter();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = string.Empty;

            var result = await cspReportBodyFormatter.ReadRequestBodyAsync(CreateInputFormatterContext(
                typeof(CspPost), httpContext));

            Assert.True(result.HasError);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_GivenValidContext_ExpectSuccessfulResultWithData()
        {
            var csp = new CspPost
            {
                CspReport = new CspReport
                {
                    Referrer = "referrer",
                    BlockedUri = "blocked-uri",
                    SourceFile = "source-file",
                },
            };

            var cspReportBodyFormatter = new CspReportBodyFormatter();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/csp-report";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(csp)));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            var result = await cspReportBodyFormatter.ReadRequestBodyAsync(CreateInputFormatterContext(
                typeof(CspPost), httpContext));

            Assert.False(result.HasError);
            Assert.IsType<CspPost>(result.Model);
        }

        private static InputFormatterContext CreateInputFormatterContext(
            Type modelType,
            HttpContext httpContext,
            string modelName = null,
            bool treatEmptyInputAsDefaultValue = false)
        {
            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForType(modelType);

            return new InputFormatterContext(
                httpContext,
                modelName ?? string.Empty,
                new ModelStateDictionary(),
                metadata,
                new TestHttpRequestStreamReaderFactory().CreateReader,
                treatEmptyInputAsDefaultValue);
        }

        private class TestHttpRequestStreamReaderFactory : IHttpRequestStreamReaderFactory
        {
            public TextReader CreateReader(Stream stream, Encoding encoding)
            {
                return new HttpRequestStreamReader(stream, encoding);
            }
        }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure.Formatters
{
    public sealed class CspReport
    {
        [JsonProperty("document-uri")]
        public string DocumentUri { get; set; }

        [JsonProperty("referrer")]
        public string Referrer { get; set; }

        [JsonProperty("effective-directive")]
        public string EffectiveDirective { get; set; }

        [JsonProperty("violated-directive")]
        public string ViolatedDirective { get; set; }

        [JsonProperty("original-policy")]
        public string OriginalPolicy { get; set; }

        [JsonProperty("blocked-uri")]
        public string BlockedUri { get; set; }

        [JsonProperty("source-file")]
        public string SourceFile { get; set; }

        [JsonProperty("line-number")]
        public int LineNumber { get; set; }

        [JsonProperty("column-number")]
        public int ColumnNumber { get; set; }

        [JsonProperty("status-code")]
        public string StatusCode { get; set; }
    }
}
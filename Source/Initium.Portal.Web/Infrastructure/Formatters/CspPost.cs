// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure.Formatters
{
    public sealed class CspPost
    {
        [JsonProperty("csp-report")]
        public CspReport CspReport { get; set; }
    }
}
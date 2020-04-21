// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Stance.Web.Infrastructure.Formatters
{
    public sealed class CspPost
    {
        [JsonProperty("csp-report")]
        public CspReport CspReport { get; set; }
    }
}
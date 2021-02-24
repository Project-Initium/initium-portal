// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Initium.Portal.Web.Infrastructure.OData.Endpoints
{
    public class ExportableFilter<TODataFilter>
        where TODataFilter : IODataFilter
    {
        public ExportableFilter()
        {
            this.Mappings = new Dictionary<string, string>();
        }

        public TODataFilter Filter { get; set; }

        public Dictionary<string, string> Mappings { get; set; }
    }
}
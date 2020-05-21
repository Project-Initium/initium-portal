// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Dynamic.Entities
{
    public class SystemAlert : ReadEntity
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public SystemAlertType Type { get; set; }

        public DateTime? WhenToShow { get; set; }

        public DateTime? WhenToHide { get; set; }
    }
}
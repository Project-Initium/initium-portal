// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class SystemAlertReadEntity : ReadEntity
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Message { get; private set; }

        public SystemAlertType Type { get; private set; }

        public DateTime? WhenToShow { get; private set; }

        public DateTime? WhenToHide { get; private set; }

        public bool IsActive { get; private set; }
    }
}
// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Queries.Models
{
    public sealed class StatusCheckModel
    {
        public StatusCheckModel(bool isPresent)
        {
            this.IsPresent = isPresent;
        }

        public bool IsPresent { get; }
    }
}
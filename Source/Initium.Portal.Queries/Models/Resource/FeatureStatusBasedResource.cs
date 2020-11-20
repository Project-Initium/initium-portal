// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Models.Resource
{
    public class FeatureStatusBasedResource
    {
        public FeatureStatusBasedResource(Guid id, bool isEnabled)
        {
            this.Id = id;
            this.IsEnabled = isEnabled;
        }

        public Guid Id { get; }

        public bool IsEnabled { get; }
    }
}
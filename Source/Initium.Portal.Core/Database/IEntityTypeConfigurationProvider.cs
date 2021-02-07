// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Core.Database
{
    public interface IEntityTypeConfigurationProvider
    {
        void ApplyConfigurations(ModelBuilder modelBuilder);
    }
}
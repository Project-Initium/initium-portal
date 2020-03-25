// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Stance.Tests")]

namespace Stance.Queries
{
    public class QuerySettings
    {
        public string ConnectionString { get; set; }
    }
}
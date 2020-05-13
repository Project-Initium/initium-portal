// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Data;

namespace Initium.Portal.Queries.Contracts.Static
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
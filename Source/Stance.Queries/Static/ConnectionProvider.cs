// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using Stance.Queries.Contracts.Static;

namespace Stance.Queries.Static
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly QuerySettings _querySettings;

        public ConnectionProvider(IOptions<QuerySettings> querySettings)
        {
            this._querySettings = querySettings.Value;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(this._querySettings.ConnectionString);
        }
    }
}
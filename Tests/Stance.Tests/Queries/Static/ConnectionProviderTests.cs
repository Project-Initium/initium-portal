// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using Moq;
using Stance.Queries;
using Stance.Queries.Static;
using Xunit;

namespace Stance.Tests.Queries.Static
{
    public class ConnectionProviderTests
    {
        [Fact]
        public void GetConnection_GivenValidConnection_ExpectSqlConnection()
        {
            var databaseSettings = new Mock<IOptions<QuerySettings>>();
            databaseSettings.Setup(x => x.Value).Returns(new QuerySettings
            {
                ConnectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;",
            });
            var adminConnectionProvider = new ConnectionProvider(databaseSettings.Object);
            var connection = adminConnectionProvider.GetConnection();
            Assert.IsType<SqlConnection>(connection);
        }
    }
}
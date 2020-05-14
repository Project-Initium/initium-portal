// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Data.SqlClient;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Static;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries.Static
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
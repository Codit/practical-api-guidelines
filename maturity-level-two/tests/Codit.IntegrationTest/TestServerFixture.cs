using Codit.LevelTwo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Codit.IntegrationTest
{
    public class TestServerFixture
    {
        internal readonly HttpClient _httpClient;

        public TestServerFixture()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _httpClient = srv.CreateClient();

        }
    }

    [CollectionDefinition("TestServer")]
    public class TestServerCollection : ICollectionFixture<TestServerFixture>
    {
    }


}

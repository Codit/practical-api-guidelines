using Codit.LevelTwo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Xunit;

namespace Codit.IntegrationTest
{
    public class TestServerFixture
    {
        internal readonly HttpClient Http;

        public TestServerFixture()
        {
            if (Http != null)
            {
                return;
            }

            var srv = new TestServer(new WebHostBuilder()
                                     
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            Http = srv.CreateClient();
        }
    }

    [CollectionDefinition("TestServer")]
    public class TestServerCollection : ICollectionFixture<TestServerFixture>
    {
    }
}

using Codit.LevelTwo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Codit.IntegrationTest
{
    public class TestServerFixture
    {
        private readonly HttpClient _httpClient;

        public TestServerFixture()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _httpClient = srv.CreateClient();

        }
    }


}

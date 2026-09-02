using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RosterVideo.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HomePage_Returns_OK()
        {
            var client = _factory.CreateClient();
            var res = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var content = await res.Content.ReadAsStringAsync();
            Assert.Contains("Submit your entry", content);
        }

        [Fact]
        public async Task ApiRoster_Returns_Json()
        {
            var client = _factory.CreateClient();
            var res = await client.GetAsync("/api/roster");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal("application/json", res.Content.Headers.ContentType?.MediaType);
        }
    }
}

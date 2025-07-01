using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestInputValidation
    {
        private HttpClient _client;
        private const string BaseUrl = "http://localhost:5203";

        [OneTimeSetUp]
        public void Setup()
        {
            _client = new HttpClient();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task SQLInjection_ShouldNotSucceed()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "admin'; DROP TABLE Users;--"),
                new KeyValuePair<string, string>("email", "attacker@example.com")
            });

            var response = await _client.PostAsync($"{BaseUrl}/submit", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.That(responseBody.ToLower(), Does.Not.Contain("drop table"));
        }

        [Test]
        public async Task XSSPayload_ShouldBeSanitized()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "<script>alert('XSS')</script>"),
                new KeyValuePair<string, string>("email", "xss@example.com")
            });

            var response = await _client.PostAsync($"{BaseUrl}/submit", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.That(responseBody, Does.Not.Contain("<script>"));
            Assert.That(responseBody, Does.Not.Contain("alert"));
        }
    }
}
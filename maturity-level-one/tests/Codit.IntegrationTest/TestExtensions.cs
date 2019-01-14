using Codit.LevelOne.Extensions;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Codit.IntegrationTest
{
    public static class TestExtensions
    {
        /// <summary>
        /// Build the HttpRequestMessage
        /// </summary>
        /// <param name="data">Object instance to be serialized</param>
        /// <param name="method">HTTP method</param>
        /// <param name="requestUri">Request URI</param>
        /// <param name="contentType">Content Type of the request</param>
        /// <returns></returns>
        public static HttpRequestMessage GetJsonRequest(object data, string method, string requestUri, string contentType = MediaTypeNames.Application.Json)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(serializedData));
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var request = new HttpRequestMessage(new HttpMethod(method), requestUri);
            request.Content = content;

            return request;
        }

        /// <summary>
        /// Validate the Length and the ContentType headers
        /// </summary>
        /// <param name="response">instance of HttpResponseMessage</param>
        /// <param name="mediaType">Expected media type</param>
        public static void ShouldBeNotNull(this HttpResponseMessage response, string mediaType = MediaTypeNames.Application.Json)
        {
            response.Content.Headers.ContentLength.Should().NotBeNull();
            response.Content.Headers.ContentType.MediaType.Should().Be(mediaType);
        }

        /// <summary>
        /// Validate the content of a HttpResponseMessage
        /// </summary>
        /// <param name="response">instance of HttpResponseMessage</param>
        /// <param name="mediaType">Expected media type</param>
        /// <param name="expectedJson">Expected json content</param>
        public static void ShouldContainContent(this HttpResponseMessage response, string mediaType, string expectedJson)
        {
            response.Content.Headers.ContentLength.Should().NotBeNull();
            response.Content.Headers.ContentType.MediaType.Should().Be(mediaType);

            string serializeObject = JsonConvert.SerializeObject(response.Content.ReadAsAsync<dynamic>().Result);
            serializeObject.Should().Be(expectedJson);
        }

        /// <summary>
        /// Validate whether a HttpResponseMessage instance is a valid problem+json
        /// </summary>
        /// <param name="response">instance of HttpResponseMessage</param>
        public static void ShouldBeProblemJson(this HttpResponseMessage response)
        {
            response.ShouldBeNotNull("application/problem+json");
            var problemJsonResponse = JsonConvert.DeserializeObject<ProblemDetailsError>(response.Content.ReadAsStringAsync().Result);
            problemJsonResponse.Status.Should().BeGreaterThan(100);
            problemJsonResponse.Title.Should().NotBeNullOrEmpty();
            problemJsonResponse.Instance.Should().NotBeNullOrEmpty();
        }
    }

}

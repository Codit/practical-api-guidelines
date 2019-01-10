using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Codit.IntegrationTest
{
    public static class TestExtensions
    {
        public static HttpRequestMessage GetJsonRequest(object data, string method, string requestUri, string contentType = "application/json")
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(serializedData));
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var request = new HttpRequestMessage(new HttpMethod(method), requestUri);
            request.Content = content;

            return request;
        }

        public static void ShouldBeNotNull(this HttpResponseMessage response, string mimeType="application/json")
        {
            response.Content.Should().NotBeNull();
            response.Content.Headers.ContentType.MediaType.Should().Be(mimeType);
        }

        public static void ShouldContainContent(this HttpResponseMessage response, string mimeType, string expectedJson)
        {
            response.Content.Should().NotBeNull();
            response.Content.Headers.ContentType.MediaType.Should().Be(mimeType);

            string serializeObject = JsonConvert.SerializeObject(response.Content.ReadAsAsync<dynamic>().Result);
            serializeObject.Should().Be(expectedJson);
        }
    }

}

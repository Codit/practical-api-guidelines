using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Codit.IntegrationTest
{
    public static class TestUtils
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
    }
}

using System;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Codit.LevelOne.Extensions
{
    public static class HttpExtensions
    {
        private static readonly JsonSerializer serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        ///     Determines if the request was made locally or not
        /// </summary>
        public static bool IsLocalRequest(this HttpRequest request)
        {
            var hostName = request?.Host.Host;
            if (string.IsNullOrWhiteSpace(hostName))
            {
                return false;
            }

            return hostName.Equals(value: "localhost", comparisonType: StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Write a JSON response
        /// </summary>
        /// <typeparam name="TResponse">Type of the response message to serizalize</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="responseObject">Response object to serialize</param>
        /// <param name="contentType">Content type</param>
        public static void WriteJson<TResponse>(this HttpResponse response, TResponse responseObject, string contentType = null)
        {
            response.ContentType = contentType ?? ContentTypeNames.Application.Json;
            using (var writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.CloseOutput = false;
                    jsonWriter.AutoCompleteOnClose = false;

                    serializer.Serialize(jsonWriter, responseObject);
                }
            }
        }
    }
}
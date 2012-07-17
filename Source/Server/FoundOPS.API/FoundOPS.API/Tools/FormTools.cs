using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FoundOPS.API.Tools
{
    public static class FormTools
    {
        /// <summary>
        /// Reads a Multipart form request, returns a organized dictionary with the HttpContent pieces seperated by key.
        /// If any keys do not exist, it will throw an exception.
        /// </summary>
        /// <param name="request">The HttpRequestMessage</param>
        /// <param name="keys">The form value names</param>
        public static Task<Dictionary<string, HttpContent>> ReadMultipartAsync(this HttpRequestMessage request, string[] keys)
        {
            var result = new Dictionary<string, HttpContent>();

            // Check if the request contains multipart/form-data.
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            // Read the form data and return an async task.
            return request.Content.ReadAsMultipartAsync()
                   .ContinueWith(t =>
                   {
                       var formContents = t.Result;
                       foreach (var key in keys)
                       {
                           var content = formContents.FirstDispositionNameOrDefault(key);

                           if (content == null)
                               throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, key + " was not sent"));

                           result.Add(key, content);
                       }

                       return result;
                   });
        }
    }
}
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FoundOps.Api.Tools
{
    public static class ApiExceptions
    {
        /// <summary>
        /// A response for when an entity was expected but could not be found
        /// </summary>
        /// <param name="request">The request to respond to</param>
        /// <param name="missingEntityName">The entity type that could not be found, ex: Status. 
        /// Defaults to Entity which should be assumed to be the current serviced entity
        /// </param>
        public static HttpResponseMessage NotFound(this HttpRequestMessage request, string missingEntityName = "Entity")
        {
            return request.CreateResponse(HttpStatusCode.NotFound, missingEntityName + " not found");
        }

        /// <summary>
        /// A response for when the current user is not authorized to perform an action
        /// </summary>
        /// <param name="request">The request to respond to</param>
        public static HttpResponseMessage NotAuthorized(this HttpRequestMessage request)
        {
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            throw new HttpResponseException(response);
        }

        /// <summary>
        /// Check if the user is authenticated. Throw an exception if they are not
        /// </summary>
        public static void CheckAuthentication(this HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Forbidden, "Not authenticated");
                throw new HttpResponseException(response);
            }
        }
    }
}

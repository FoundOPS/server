using System.Net;
using System.Net.Http;

namespace FoundOPS.Api.Tools
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
            //no need to be descriptive yet
            return request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
        }
    }
}

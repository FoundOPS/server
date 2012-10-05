using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Tools;

namespace FoundOps.Api.Tools
{
    public static class ApiExceptions
    {
        #region Common Exceptions

        /// <summary>
        /// A response for when the current user is not authorized to perform an action
        /// </summary>
        /// <param name="request">The request to respond to</param>
        public static HttpResponseException NotAuthorized(this HttpRequestMessage request)
        {
            return new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized"));
        }

        /// <summary>
        /// A response for when an entity was expected but could not be found
        /// </summary>
        /// <param name="request">The request to respond to</param>
        /// <param name="missingEntityName">The entity type that could not be found, ex: Status. 
        /// Defaults to Entity which should be assumed to be the current serviced entity
        /// </param>
        public static HttpResponseException NotFound(this HttpRequestMessage request, string missingEntityName = "Entity")
        {
            return new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, missingEntityName + " not found"));
        }

        /// <summary>
        /// An error when saving
        /// </summary>
        public static HttpResponseException NotSaving(this HttpRequestMessage request)
        {
            return new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, "Error saving"));
        } 

        #endregion

        //Tools

        /// <summary>
        /// Check if the user is authenticated. Throw an exception if they are not
        /// </summary>
        public static void CheckAuthentication(this HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(AuthenticationLogic.CurrentUsersEmail()))
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, "Not authenticated"));
        }
    }
}

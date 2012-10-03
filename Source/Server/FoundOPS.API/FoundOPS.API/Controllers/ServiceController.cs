using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FoundOps.Common.NET;

namespace FoundOPS.Api.Controllers
{
    public class ServiceController : Controller
    {
        /// <summary>
        /// Get service holders with fields 
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="clientContext">The Id of the client to filter by</param>
        /// <param name="recurringServiceContext">The Id of the recurring service to filter by</param>
        /// <param name="startDate">The start date (in the user's time zone)</param>
        /// <param name="endDate">The end date (in the user's time zone)</param>
        /// <param name="serviceType">The service type to filter by</param>
        /// <param name="single">Only return the types</param>
        /// <returns>A queryable of dictionaries that resemble record type javascript objects when serialized</returns>
        public ActionResult GetServicesHoldersWithFieldsCsv(Guid roleId, string serviceType, Guid? clientContext,
            Guid? recurringServiceContext, DateTime startDate, DateTime endDate, bool single = false)
        {
            var serviceController = new Api.ServiceController();
            var response = serviceController.GetServicesHoldersWithFields(roleId, serviceType, clientContext, recurringServiceContext, startDate, endDate);

            var ignore = new[] { "ServiceId", "ClientId", "RecurringServiceId" };

            var headers = (from kvp in response.Skip(1).First()
                           where !ignore.Contains(kvp.Key)
                           select kvp.Key.Replace("_", " "))
                           .ToArray();

            return File(Encoding.UTF8.GetBytes(response.Skip(1).ToCSV(headers, ignore)), "text/csv", "Services.csv");
        }
    }
}
﻿using FoundOps.Common.NET;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoundOps.Api.Controllers.Mvc
{
    public class ServiceHoldersController : Controller
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
        public ActionResult GetCsv(Guid roleId, string serviceType, Guid? clientContext,
            Guid? recurringServiceContext, DateTime startDate, DateTime endDate, bool single = false)
        {
            var serviceHolderController = new Rest.ServiceHoldersController();
            var response = serviceHolderController.Get(roleId, serviceType, clientContext, recurringServiceContext, startDate, endDate);

            var ignore = new[] { "ServiceId", "ClientId", "RecurringServiceId" };

            var headers = (from kvp in response.Skip(1).FirstOrDefault()
                           where !ignore.Contains(kvp.Key)
                           select kvp.Key.Replace("_", " "))
                           .ToArray();

            return File(Encoding.UTF8.GetBytes(response.Skip(1).ToCSV(headers, ignore)), "text/csv", "Services.csv");
        }
    }
}
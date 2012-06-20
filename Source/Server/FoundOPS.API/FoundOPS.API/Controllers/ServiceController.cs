using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FoundOPS.API.Models;

namespace FoundOPS.API.Controllers
{
    public class ServiceController
    {
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Service> GetServices(Guid roleId)
    }
}
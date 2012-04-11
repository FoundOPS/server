using System;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class WorkFlowController : Controller
    {
        public ActionResult TimeCheck()
        {
            if (!(DateTime.UtcNow == new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2, 0, 0)))
                return View();

            //TODO: trigger Workflow
            return View();
        }

    }
}
using System;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class WorkFlowController : Controller
    {
        public ActionResult TimeCheck()
        {
            if (!(DateTime.Now == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 0, 0)))
                return View();

            //TODO: trigger Workflow
            return View();
        }

    }
}
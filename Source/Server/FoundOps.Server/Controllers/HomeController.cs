using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/App/Index");
        }

        [FoundOps.Server.Tools.Authorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}
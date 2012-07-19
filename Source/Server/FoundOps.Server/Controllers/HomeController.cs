using FoundOps.Server.Tools;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/App/Index");
        }

        [AddTestUsersThenAuthorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}
using FoundOps.Core.Models;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect(AppConstants.ApplicationUrl);
        }
    }
}
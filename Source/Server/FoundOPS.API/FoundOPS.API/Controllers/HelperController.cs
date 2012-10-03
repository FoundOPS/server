using System.Text;
using System.Web.Mvc;

namespace FoundOPS.Api.Controllers
{
    public class HelperController : Controller
    {
        /// <summary>
        /// For client side file creation
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="contentType">The content type</param>
        /// <param name="fileName">The file name</param>
        /// <returns>A file</returns>
        public ActionResult Download(string content, string contentType, string fileName)
        {
            return File(Encoding.UTF8.GetBytes(content), "text/csv", fileName);
        }
    }
}

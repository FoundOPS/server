using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using FoundOps.Core.Server;
using FoundOps.Server.Tools;
using FoundOps.Common.Server;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (ControllerContext.IsMobileDevice())
            //{
            //    var action = HttpContext.User.Identity.IsAuthenticated ? "Index" : "Login";
            //    return RedirectToAction(action, "M", null);
            //}

#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan") || UserSpecificResourcesWrapper.GetBool("AutomaticLoginDavid"))
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View();
            }

            return RedirectToAction("Silverlight", "Home");
        }

        [AddTestUsersThenAuthorize]
        public ActionResult Silverlight()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://fstore.blob.core.windows.net/xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();

            //Must cast the string as an object so it uses the correct overloaded method
            return View((object)version);
        }

        public ActionResult Team(string id)
        {
            return Redirect(String.Format("{0}/Team?id={1}", Global.RootBlogUrl, id));
        }

        /// <summary>
        /// Redirects to the respective wordpress page
        /// </summary>
        /// <param name="page">The page to redirect to.</param>
        public ActionResult WP(string page)
        {
            if (page.ToLower() == "blog")
                page = ""; //wp.foundops.com

            return Redirect(String.Format("{0}/{1}", Global.RootBlogUrl, page));
        }

        public ContentResult Sitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            //Sitemap Items. If the item is not on the server do not set a FilePath and make sure to hardcode the LastModified date
            var items = new[]
                            {
                                new
                                    {
                                        Global.RootUrl,
                                        UrlPath= "Home/Index",
                                        FilePath = @"..\Views\Home\Index.cshtml",
                                        LastModified = new DateTime?(),
                                        Priority = "1.0"
                                    },
                                new
                                    {
                                        Global.RootUrl,
                                        UrlPath= "Home/Product",
                                        FilePath = @"..\Views\Home\Product.cshtml",
                                        LastModified = new DateTime?(),
                                        Priority = "0.8"
                                    },
                                new
                                    {
                                        Global.RootUrl,
                                        UrlPath= "Home/ContactUs",
                                        FilePath = @"..\Views\Home\ContactUs.cshtml",
                                        LastModified = new DateTime?(),
                                        Priority = "0.6"
                                    },
                                new
                                    {
                                        RootUrl = Global.RootBlogUrl,
                                        UrlPath= "team",
                                        FilePath = "",
                                        LastModified = new DateTime?(new DateTime(2011, 10, 1)),
                                        Priority = "0.4"
                                    },
                                new
                                    {
                                        RootUrl = Global.RootBlogUrl,
                                        UrlPath= "values",
                                        FilePath = "",
                                        LastModified = new DateTime?(new DateTime(2011, 10, 1)),
                                        Priority = "0.4"
                                    },
                                new
                                    {
                                        RootUrl = Global.RootBlogUrl,
                                        UrlPath= "beta",
                                        FilePath = "",
                                        LastModified = new DateTime?(new DateTime(2011, 10, 1)),
                                        Priority = "0.5"
                                    },
                                new
                                    {
                                        RootUrl = Global.RootBlogUrl,
                                        UrlPath= "jobs",
                                        FilePath = "",
                                        LastModified = new DateTime?(new DateTime(2011, 10, 1)),
                                        Priority = "0.2"
                                    },
                                new
                                    {
                                        RootUrl = Global.RootBlogUrl,
                                        UrlPath= "",
                                        FilePath = "",
                                        LastModified = new DateTime?(new DateTime(2011, 10, 1)),
                                        Priority = "0.6"
                                    }
                            };

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in items
                    select
                        //Add ns to every element.
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Format("{0}/{1}", i.RootUrl, i.UrlPath)),
                        //If the FilePath does not have a hardcoded value return the hardcoded LastModified date
                        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", string.IsNullOrEmpty(i.FilePath) ? i.LastModified : GetLastWriteTime(i.FilePath))),
                        new XElement(ns + "changefreq", "monthly"),
                        new XElement(ns + "priority", i.Priority)
                        )
                    )
                );
            return Content(sitemap.ToString(), "text/xml");
        }

        private DateTime GetLastWriteTime(string relativeFilePath)
        {
            var filePath = HttpContext.Server.MapPath(relativeFilePath);
            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            return lastModified;
        }

#if DEBUG

        /// <summary>
        /// Clear, create database, and populate design data.
        /// </summary>
        /// <returns></returns>
        public ActionResult CCDAPDD()
        {
#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan") ||
                UserSpecificResourcesWrapper.GetBool("AutomaticLoginDavid") || UserSpecificResourcesWrapper.GetBool("TestServer"))
            {
                CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
                return View();
            }
#endif

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        [AddTestUsersThenAuthorize]
        public ActionResult PerformServerOperations()
        {
            //TODO Authenticate first
            ServerManagement.PerformServerOperations();
            return View();
        }
#endif
    }
}
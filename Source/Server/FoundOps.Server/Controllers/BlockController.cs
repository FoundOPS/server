using System;
using System.IO;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class BlockController : Controller
    {
        //TODO: Add authentication metadata
        public ActionResult GetBlock(string id)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Panes";

            //var currentUserHasAccess = CoreEntitiesLogic.CurrentUserHasAccessToBlock(id);

            //if (currentUserHasAccess)
           
#if DEBUG
            return File(Path.Combine(path, id), "application/x-silverlight-2", id);
#else
            var request = (HttpWebRequest)WebRequest.Create(String.Format("http://fstore.blob.core.windows.net/xaps/{0}", id));
            var response = (HttpWebResponse)request.GetResponse();
            return File(response.GetResponseStream(), "application/x-silverlight-2", id);
#endif
        }
    }
}

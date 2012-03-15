using System;
using System.Web.Mvc;
using FoundOps.Common.Server;

namespace FoundOps.Core.Server.Tools
{
    public static class MobileHelpers
    {
        public static bool UserAgentContains(this ControllerContext c, string agentToFind)
        {
            return (c.HttpContext.Request.UserAgent.IndexOf(agentToFind, StringComparison.OrdinalIgnoreCase) > 0);
        }

        public static bool IsMobileDevice(this ControllerContext c)
        {
#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("TestingMobile"))
                return true;
#endif

            return c.HttpContext.Request.Browser.IsMobileDevice;
        }
    }
}
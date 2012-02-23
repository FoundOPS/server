using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Text.RegularExpressions;

namespace AbsoluteRouting
{
    public class AbsoluteUrlRoutingModule : UrlRoutingModule
    {
        public override void PostMapRequestHandler(HttpContextBase context)
        {
            base.PostMapRequestHandler(new HttpsAwareHttpContextWrapper(HttpContext.Current));
        }
        public override void PostResolveRequestCache(HttpContextBase context)
        {
            base.PostResolveRequestCache(new HttpsAwareHttpContextWrapper(HttpContext.Current));
        }
        private class HttpsAwareHttpContextWrapper : HttpContextWrapper
        {
            HttpContext _context;
            public HttpsAwareHttpContextWrapper(HttpContext httpContext)
                : base(httpContext)
            {
                _context = httpContext;
            }
            public override HttpResponseBase Response
            {
                get { 
                    return new HttpsAwareHttpResponseWrapper(_context.Response, Request.ApplicationPath); 
                }
            }
            private class HttpsAwareHttpResponseWrapper : HttpResponseWrapper
            {
                string _appPath;
                public HttpsAwareHttpResponseWrapper(HttpResponse response, string appPath)
                    : base(response)
                {
                    _appPath = appPath;
                    if (!_appPath.EndsWith("/")) _appPath += "/";
                }
                public override string ApplyAppPathModifier(string virtualPath)
                {
                    if (Regex.IsMatch(virtualPath, _appPath + "https?:"))
                        return virtualPath.Substring(_appPath.Length);
                    else
                        return base.ApplyAppPathModifier(virtualPath);
                }
            }
        }
    }
}

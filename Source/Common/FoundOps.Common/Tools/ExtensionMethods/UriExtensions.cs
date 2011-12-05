using System;
using System.Windows.Browser;

namespace FoundOps.Common.Tools.ExtensionMethods
{
    public static class UriExtensions
    {
        /// <summary>
        /// Ex. http://localhost:3333/Someplace/Somewhere -> http://localhost:3333
        /// </summary>
        public static string ThisRootUrl = HtmlPage.Document.DocumentUri.RootUrl();

        /// <summary>
        /// Ex. http://localhost:3333/Someplace/Somewhere -> http://localhost:3333
        /// </summary>
        public static string RootUrl(this Uri uri)
        {
            return uri.ToString().Remove(uri.ToString().IndexOf(uri.AbsolutePath));
        }
    }
}

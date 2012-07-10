using System;
using System.Windows.Browser;
using System.ComponentModel.Composition;
using FoundOps.Common.Tools.ExtensionMethods;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.Data
{
    /// <summary>
    /// Exports a common CoreDomainContext to be shared among the application.
    /// </summary>
    public class ExportContext
    {
        /// <summary>
        /// The CoreDomainContext to share among the application.
        /// </summary>
        [Export]
        public CoreDomainContext CoreDomainContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportContext"/> class.
        /// </summary>
        public ExportContext()
        {
            //Manually set the rootUrl because it is different for debugging and deployement

#if DEBUGNODE
            //The silverlight app will be running under the node server, different from the DomainService
            var rootUrl = "http://localhost:31820";
#else
            //The rootUrl for the domainService
            //In deployement it is the same as the root url of the server (which is the current document's root url)
            var rootUrl = HtmlPage.Document.DocumentUri.RootUrl();
#endif

#if DEBUG
            //In debug mode, on the local server, the CoreDomainContext is under ClientBin
            rootUrl = rootUrl + "/ClientBin";
#else
            //In testrelease and release mode setup the endpoint to be HTTPS
           if(!rootUrl.Contains("https"))
               rootUrl = rootUrl.Replace("http", "https");
#endif

            var serviceUrl = new Uri(String.Format("{0}/{1}", rootUrl, "FoundOps-Server-Services-CoreDomainService-CoreDomainService.svc"), UriKind.RelativeOrAbsolute);

            CoreDomainContext = new CoreDomainContext(serviceUrl);

            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout = new TimeSpan(0, 10, 0);
        }
    }
}

using System;
using System.Windows.Browser;
using System.ComponentModel.Composition;
using FoundOps.Common.Tools.ExtensionMethods;
using System.ServiceModel.DomainServices.Client;
using FoundOps.SLClient.Data.Tools;
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
            var domainServiceUrl = ClientConstants.RootDomainServiceUrl;

            var serviceUrl = new Uri(String.Format("{0}/{1}", domainServiceUrl, "FoundOps-Server-Services-CoreDomainService-CoreDomainService.svc"), UriKind.RelativeOrAbsolute);

            CoreDomainContext = new CoreDomainContext(serviceUrl);

            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            ((WebDomainClient<CoreDomainContext.ICoreDomainServiceContract>)CoreDomainContext.DomainClient).ChannelFactory.Endpoint.Binding.SendTimeout = new TimeSpan(0, 10, 0);
        }
    }
}

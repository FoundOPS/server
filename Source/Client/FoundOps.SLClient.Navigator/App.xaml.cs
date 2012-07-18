using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Navigator.Controls;
using MEFedMVVM.ViewModelLocator;
using RiaServicesContrib.DataValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace FoundOps.SLClient.Navigator
{
    public partial class App : IComposer
    {
        private CompositionContainer _container;
        public App()
        {
            this.Startup += this.ApplicationStartup;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //Required for MEF
            LocatorBootstrapper.ApplyComposer(this);

            //Required for EntityFramework Validation
            MEFValidationRules.RegisterAssembly(typeof(LocationField).Assembly);

            #region Add IP Info to Resources

            var ipAddressLocationQuery = String.Format("http://api.ipinfodb.com/v3/ip-city/?key={0}", "50191ba897c5677bc6a49f46f5da10787c7898f34b8a11d8e1c01546b8a08470");
            var ipGeocoderService = new WebClient();
            ipGeocoderService.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error != null) return;
                Current.Resources.Add("IPInformation", args.Result);
            };
            ipGeocoderService.DownloadStringAsync(new Uri(ipAddressLocationQuery));

            #endregion

            this.RootVisual = new TextBlock(); //To Prevent ThreadableVM from having issues, first load another rootvisual
            this.RootVisual = new MainPage();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled.
                // For production applications this error handling should be replaced with something that will
                // report the error to the website and stop the application.
                e.Handled = true;

#if! RELEASE //(DEBUG or TESTRELEASE)
                ErrorWindow.CreateNew(e.ExceptionObject);
#endif

                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ReportErrorToDOM(e);
                });
            }



            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;


            //}
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("console.log(" + errorMsg + ");");

                //System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        #region Implementation of IComposer

        public IEnumerable<ExportProvider> GetCustomExportProviders()
        {
            return null;
        }

        ComposablePartCatalog IComposer.InitializeContainer()
        {
            var aggregateCatalog = new AggregateCatalog();

            //Deployment catalog adds all assemblies, but FoundOps.SLClient.Algorithm cannot be added
            //so instead only add required assemblies through AssemblyCatalog
            var commonUIAssembly = new AssemblyCatalog(typeof(Common.Silverlight.UI.Controls.InfiniteAccordion.IProvideContext).Assembly);
            var dataAssembly = new AssemblyCatalog(typeof(Data.ExportContext).Assembly);
            var uiAssembly = new AssemblyCatalog(typeof(UI.ViewModels.ClientsVM).Assembly);
            var navigatorAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());

            aggregateCatalog.Catalogs.Add(commonUIAssembly);
            aggregateCatalog.Catalogs.Add(dataAssembly);
            aggregateCatalog.Catalogs.Add(uiAssembly);
            aggregateCatalog.Catalogs.Add(navigatorAssembly);

            _container = new CompositionContainer(aggregateCatalog);

            CompositionHost.Initialize(_container);

            return _container.Catalog;
        }

        #endregion
    }
}

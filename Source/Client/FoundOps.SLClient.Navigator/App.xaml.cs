﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Core.Navigator.Loader;
using FoundOps.Core.Navigator.Controls;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using RiaServicesContrib.DataValidation;

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
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
#if! RELEASE //(DEBUG or TESTRELEASE)
            ErrorWindow.CreateNew(e.ExceptionObject);
#endif

            //}
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
            var commonUIAssembly = new AssemblyCatalog(typeof(FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion.IProvideContext).Assembly);
            var dataAssembly = new AssemblyCatalog(typeof(FoundOps.SLClient.Data.ExportContext).Assembly);
            var uiAssembly = new AssemblyCatalog(typeof(FoundOps.SLClient.UI.ViewModels.AlgorithmVM).Assembly);
            var navigatorAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());

            aggregateCatalog.Catalogs.Add(commonUIAssembly);
            aggregateCatalog.Catalogs.Add(dataAssembly);
            aggregateCatalog.Catalogs.Add(uiAssembly);
            aggregateCatalog.Catalogs.Add(navigatorAssembly);


            _container = new CompositionContainer(aggregateCatalog);

            //Adds aggregateCatalogService as an export, available to any composed parts in the container
            var aggregateCatalogService = new AggregateCatalogService(aggregateCatalog, _container);

            _container.ComposeParts(aggregateCatalogService);

            CompositionHost.Initialize(_container);

            return _container.Catalog;
        }

        #endregion
    }
}

using System;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.UI.Controls.Printing;
using FoundOps.Common.Tools;
using GalaSoft.MvvmLight.Command;
using FoundOps.SLClient.Data.Models;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying and printing RouteManifests
    /// </summary>
    public class RouteManifestVM : CoreEntityVM
    {
        #region Public

        /// <summary>
        /// The printer.
        /// </summary>
        public IPagedPrinter Printer { get; set; }

        /// <summary>
        /// Gets the route manifest settings.
        /// </summary>
        public RouteManifestSettings RouteManifestSettings { get; private set; }

        #region Commands

        /// <summary>
        /// Go forward one page in the manifest.
        /// </summary>
        public RelayCommand ForwardOnePage { get; private set; }

        /// <summary>
        /// Go back one page in the manifest.
        /// </summary>
        public RelayCommand BackOnePage { get; private set; }

        /// <summary>
        /// Go to the first page in the manifest.
        /// </summary>
        public RelayCommand GoToFirstPage { get; private set; }

        /// <summary>
        /// Go to the last page in the manifest.
        /// </summary>
        public RelayCommand GoToLastPage { get; private set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        public RouteManifestVM(DataManager dataManager)
            : base(dataManager)
        {
            //Setup the RouteManifestSettings based on the OwnerAccount
            ContextManager.OwnerAccountObservable.Select(oa => oa as BusinessAccount).Where(ba => ba != null)
                .Subscribe(currentBusinessAccount =>
            {
                //Setup the currentBusinessAccount's RouteManifestSettings if there are none
                if (currentBusinessAccount.RouteManifestSettings == null)
                {
                    currentBusinessAccount.RouteManifestSettings = SerializationTools.Serialize(new RouteManifestSettings());
                    DataManager.EnqueueSubmitOperation();
                }
                try
                {
                    //Try to use the currentBusinessAccount's RouteManifestSettings
                    RouteManifestSettings = SerializationTools.Deserialize<RouteManifestSettings>(currentBusinessAccount.RouteManifestSettings);
                }
                catch
                {
                    //If there is a problem reset the RouteManifestSettings
                    this.RouteManifestSettings = new RouteManifestSettings();
                    currentBusinessAccount.RouteManifestSettings = SerializationTools.Serialize(RouteManifestSettings);
                    DataManager.EnqueueSubmitOperation();
                }
            });

            #region Register Commands

            ForwardOnePage = new RelayCommand(() =>
            {
                //if (!Printer.IsLastPage)
                //    Printer.CurrentPageIndex++;
            });
            BackOnePage = new RelayCommand(() =>
            {
                //if (!Printer.IsFirstPage)
                //    Printer.CurrentPageIndex--;
            });
            //GoToFirstPage = new RelayCommand(() => Printer.CurrentPageIndex = 0);
            GoToLastPage = new RelayCommand(() =>
            {
                //Printer.CurrentPageIndex = Printer.PageCount - 1;
            });

            #endregion
        }

        #region RouteManifestVM's Logic

        ///<summary>
        /// Updates and saves the current business account's RouteManifestSettings
        ///</summary>
        public void UpdateSaveRouteManifestSettings()
        {
            ((BusinessAccount)this.ContextManager.OwnerAccount).RouteManifestSettings =
                SerializationTools.Serialize(this.RouteManifestSettings);

            this.SaveCommand.Execute(null);
        }

        #endregion
    }
}

using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Models;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Reactive.Linq;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying and printing RouteManifests
    /// </summary>
    [ExportViewModel("RouteManifestVM")]
    public class RouteManifestVM : CoreEntityVM
    {
        #region Public

        /// <summary>
        /// Gets the route manifest settings.
        /// </summary>
        public RouteManifestSettings RouteManifestSettings { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestVM"/> class.
        /// </summary>
        public RouteManifestVM()
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

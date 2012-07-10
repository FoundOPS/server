using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.Data.ViewModels;
using System;
using System.Reactive.Linq;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for editing a single RouteDestination
    /// </summary>
    public class RouteDestinationVM : CoreEntityVM, IDisposable
    {
        #region Public Properties

        private ContactInfoVM _clientContactInfoVM;

        /// <summary>
        /// Gets the ContactInfoVM for the selected route destination's client.
        /// </summary>
        public ContactInfoVM ClientContactInfoVM
        {
            get { return _clientContactInfoVM; }
            set
            {
                _clientContactInfoVM = value;
                this.RaisePropertyChanged("ClientContactInfoVM");
            }
        }

        private ContactInfoVM _locationContactInfoVM;

        /// <summary>
        /// Gets the ContactInfoVM for the location.
        /// </summary>
        public ContactInfoVM LocationContactInfoVM
        {
            get { return _locationContactInfoVM; }
            set
            {
                _locationContactInfoVM = value;
                this.RaisePropertyChanged("LocationContactInfoVM");
            }
        }

        /// <summary>
        /// The RouteDestination.
        /// </summary>
        public RouteDestination RouteDestination { get; set; }

        #endregion

        #region Locals

        //Keep these so we can dispose the property changed subscription
        private readonly IDisposable _destinationDisposable;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDestinationVM"/> class.
        /// </summary>
        /// <param name="routeDestination">The route destination.</param>
        public RouteDestinationVM(RouteDestination routeDestination)
        {
            RouteDestination = routeDestination;

            if (routeDestination == null)
                return;

            //Initialize the ContactInfoVMs
            UpdateContactInfoVMs();

            //Whenever the Client/Location/DetailsLoaded property changes update the VMs
            _destinationDisposable = RouteDestination.FromAnyPropertyChanged().WhereNotNull()
                .Where(p => p.PropertyName == "DetailsLoaded" || p.PropertyName == "Client" || p.PropertyName == "Location")
                .Subscribe(_ => UpdateContactInfoVMs());
        }

        #region Logic

        /// <summary>
        /// Updates the ContactInfoVMs
        /// </summary>
        private void UpdateContactInfoVMs()
        {
            ClientContactInfoVM = RouteDestination.Client != null && RouteDestination.Client != null
                                      ? new ContactInfoVM(ContactInfoType.OwnedParties, RouteDestination.Client.ContactInfoSet)
                                      : null;

            LocationContactInfoVM = RouteDestination.Location != null
                                        ? new ContactInfoVM(ContactInfoType.OwnedParties, RouteDestination.Location.ContactInfoSet)
                                        : null;
        }

        /// <summary>
        /// Disposes the ClientGraph property changed subscription and the Location subscription 
        /// </summary>
        public void Dispose()
        {
            if (_destinationDisposable != null)
                _destinationDisposable.Dispose();
        }

        #endregion
    }
}

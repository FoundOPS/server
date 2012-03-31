using System;
using System.Reactive.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.Data.ViewModels;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for editing a single RouteDestination
    /// </summary>
    public class RouteDestinationVM : CoreEntityVM
    {
        /// <summary>
        /// The RouteDestination.
        /// </summary>
        public RouteDestination RouteDestination { get; set; }

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

            //Whenever the Client changes, update the ClientContactInfoVM
            Observable2.FromPropertyChangedPattern(RouteDestination, r => r.Client)
                .ObserveOnDispatcher().Where(client => client != null && client.OwnedParty != null).Subscribe(client =>
                    ClientContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, client.OwnedParty.ContactInfoSet));

            //Whenever the Location changes, update the LocationContactInfoVM
            Observable2.FromPropertyChangedPattern(RouteDestination, r => r.Location)
                .ObserveOnDispatcher().Where(loc => loc != null).Subscribe(loc =>
                    LocationContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, loc.ContactInfoSet));
        }
    }
}

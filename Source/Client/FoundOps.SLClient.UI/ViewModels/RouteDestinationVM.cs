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

        /// <summary>
        /// Gets the ContactInfoVM for the selected route destination's client.
        /// </summary>
        public ContactInfoVM ClientContactInfoVM { get; set; }

        /// <summary>
        /// Gets the ContactInfoVM for the location.
        /// </summary>
        public ContactInfoVM LocationContactInfoVM { get; set; }

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

            //Setup the ContactInfoVMs
            if (RouteDestination.Client != null && RouteDestination.Client.OwnedParty != null)
                ClientContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, RouteDestination.Client.OwnedParty.ContactInfoSet);

            if (RouteDestination.Location != null)
            LocationContactInfoVM = new ContactInfoVM(ContactInfoType.Locations, RouteDestination.Location.ContactInfoSet);
        }
    }
}

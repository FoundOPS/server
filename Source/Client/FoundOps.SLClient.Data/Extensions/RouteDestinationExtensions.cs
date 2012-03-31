using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;
using System.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteDestination : ILoadDetails, IReject
    {
        #region Public Properties

        #region Implementation of ILoadDetails

        private bool _detailsLoaded;
        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                _detailsLoaded = value;
                this.CompositeRaiseEntityPropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        //NOTE: The notify property changed handling happens on the Server Extensions
        public string Name
        {
            get
            {
                //If this Location is null, return the first RouteTask with a LocationName
                if (Location == null)
                {
                    var routeTaskName = this.RouteTasks.Select(rt => rt.LocationName).FirstOrDefault(ln => !string.IsNullOrEmpty(ln));
                    if (routeTaskName != null)
                        return routeTaskName;
                }

                return Location != null ? this.Location.Name : "**********************";
            }
        }

        private OrderedEntityCollection<RouteTask> _routeTasksListWrapper;
        /// <summary>
        /// Wraps the RouteTasks for drag and drop to work http://www.telerik.com/community/forums/silverlight/drag-and-drop/draganddrop-with-radtreeview-and-entitycollection.aspx
        /// </summary>
        public OrderedEntityCollection<RouteTask> RouteTasksListWrapper
        {
            get { return _routeTasksListWrapper; }
            private set
            {
                _routeTasksListWrapper = value;
                this.RaisePropertyChanged("RouteTasksListWrapper");
            }
        }

        #endregion

        #region Constructor

        partial void OnInitializedSilverlight()
        {
            RouteTasksListWrapper = new OrderedEntityCollection<RouteTask>(this.RouteTasks, "OrderInRouteDestination", false);
            this.RouteTasks.EntityAdded += (s, e) => this.RaisePropertyChanged("RouteTasksListWrapper");
            this.RouteTasks.EntityRemoved += (s, e) => this.RaisePropertyChanged("RouteTasksListWrapper");
        }

        #endregion

        #region Public Methods

        public void Reject()
        {
            this.RejectChanges();
        }

        #endregion
    }
}

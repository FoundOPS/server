using System;
using FoundOps.Common.Silverlight.Models.Collections;
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
                //Cannot clear details loaded. This is prevent issues when saving.
                if (_detailsLoaded)
                    return;

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

                return Location != null ? this.Location.AddressLineOne : "**********************";
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

        private Func<RouteTask, OrderedEntityCollection<RouteTask>> GetRouteTasksListWrapper
        {
            get
            {
                return routeTask => routeTask.RouteDestination == null ? null : routeTask.RouteDestination.RouteTasksListWrapper;
            }
        }

        partial void OnInitializedSilverlight()
        {
            RouteTasksListWrapper = new OrderedEntityCollection<RouteTask>(this.RouteTasks, "OrderInRouteDestination", false, GetRouteTasksListWrapper);
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

using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.Interfaces;
using System.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteDestination : IReject
    {
        partial void OnInitializedSilverlight()
        {
            RouteTasksListWrapper = new OrderedEntityCollection<RouteTask>(this.RouteTasks, "OrderInRouteDestination", false);
            this.RouteTasks.EntityAdded += (s, e) => this.RaisePropertyChanged("RouteTasksListWrapper");
            this.RouteTasks.EntityRemoved += (s, e) => this.RaisePropertyChanged("RouteTasksListWrapper");
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

        public void Reject()
        {
            this.RejectChanges();
        }
    }
}

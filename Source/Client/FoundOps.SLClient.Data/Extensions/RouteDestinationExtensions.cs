using FoundOps.Common.Silverlight.Tools;
using Telerik.Windows.Controls;
using System.Collections.ObjectModel;
using FoundOps.Core.Context.Extensions;
using FoundOps.Common.Silverlight.Interfaces;

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
            get { return Location != null ? this.Location.Name : "**********************"; }
        }

        public ScheduleViewAppointment ScheduleViewAppointment
        {
            get
            {
                var apt = new ScheduleViewAppointment { Subject = this.Name, Location = this.Location.Name };
                var newResource = new Resource();
                newResource.ResourceName = this.Route.Name;
                newResource.ResourceType = "Route";
                apt.Resources.Add(newResource);
                //TODO: Start and End Time
                apt.Start = this.StartTime;
                apt.End = this.EndTime;
                var tasks = new ObservableCollection<RouteTask>();
                foreach (var routeTask in this.RouteTasks)
                {
                    tasks.Add(routeTask);
                    tasks.Add(routeTask);//testing code
                }
                apt.Tasks = tasks;
                return apt;
            }
        }

        public void Reject()
        {
            this.RejectChanges();
        }
    }
}

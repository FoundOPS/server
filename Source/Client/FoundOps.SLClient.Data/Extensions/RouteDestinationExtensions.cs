using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.MVVM.Interfaces;
using FoundOps.Core.Context.Extensions;
using FoundOps.Framework.Views.Models;
using Telerik.Windows.Controls;


namespace FoundOps.Core.Models.CoreEntities
{
    public partial class RouteDestination : IReject
    {
        //NOTE: The notify property changed handling happens on the Server Extensions
        public string Name
        {
            get { return Location != null ? this.Location.Name : "**********************"; }
        }

        public ScheduleViewAppointment ScheduleViewAppointment
        {
            get
            {
                var apt = new ScheduleViewAppointment { Subject = this.Name, Location = this.Location.Name};
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
        //Needs to be observable collection to allow ordering in TreeView
        public ObservableCollection<RouteTask> Tasks
        {
            get
            {
                return new ObservableCollection<RouteTask>(RouteTasks);
            }
        }

        public void Reject()
        {
            this.RejectChanges();
        }

        public void ForceTasksRefresh()
        {
            this.RaisePropertyChanged("Tasks");
        }
    }
}

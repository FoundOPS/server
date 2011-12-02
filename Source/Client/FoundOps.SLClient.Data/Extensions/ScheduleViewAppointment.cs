using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.ScheduleView;

namespace FoundOps.Core.Context.Extensions
{
    public class ScheduleViewAppointment : Appointment
    {
        //Adding the ability to store a list of Tasks to appointments. 
        //public ObservableCollection<RouteTask> Tasks { get; set; }
        public ObservableCollection<RouteTask> Tasks { get; set; }

        public ScheduleViewAppointment()
        {
            //Tasks = new ObservableCollection<RouteTask>();

        }


    }
}

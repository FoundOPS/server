using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    public partial class TaskDetails
    {
        public TaskDetails()
        {
            InitializeComponent();
        }

        private void StatusComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRouteTask = VM.Routes.SelectedRouteTask;

            var status = selectedRouteTask.TaskStatus;
            if (!status.RouteRequired)// This would mean that the status requires the task to be out of a route
            {
                var destination = selectedRouteTask.RouteDestination;

                var lastRouteTask = destination.Route.RouteDestinationsListWrapper.LastOrDefault();

                selectedRouteTask.RemoveRouteDestination();

                DragDropTools.RemoveFromRoute(selectedRouteTask);

                if (destination.RouteTasks.Count == 0)
                    VM.Routes.DeleteRouteDestination(destination);

                ((ObservableCollection<TaskHolder>)VM.TaskBoard.CollectionView.SourceCollection).Add(selectedRouteTask.ParentRouteTaskHolder);

                if (lastRouteTask != null)
                    VM.Routes.SelectedRouteTask = lastRouteTask.RouteTasks.FirstOrDefault();

                VM.Routes.DispatcherSave();
            }
        }
    }
}

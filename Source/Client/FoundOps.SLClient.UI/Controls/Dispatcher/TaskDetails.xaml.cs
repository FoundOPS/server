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

            if(selectedRouteTask == null)
                return;

            var source = e.OriginalSource as RadComboBox;

            if(source == null || !source.IsDropDownOpen)
                return;

            var status = selectedRouteTask.TaskStatus;
            if (status != null && !status.RouteRequired && e.RemovedItems.Count > 0)// This would mean that the status requires the task to be out of a route
            {
                //This will also remove the task from its RouteDestination and delete the Destination if there are no other tasks for that Destination
                DragDropTools.RemoveFromRoute(selectedRouteTask);

                ((ObservableCollection<RouteTask>)VM.TaskBoard.CollectionView.SourceCollection).Add(selectedRouteTask);

                VM.Routes.DispatcherSave();
            }
        }
    }
}

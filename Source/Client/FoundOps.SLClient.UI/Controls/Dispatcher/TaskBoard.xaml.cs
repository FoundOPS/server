using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using Telerik.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.TreeView;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays the Tasks to be routed for the day.
    /// </summary>
    public partial class TaskBoard
    {
        //Public

        /// <summary>
        /// Exposes the public task board rad grid view.
        /// </summary>
        public RadGridView PublicTaskBoardRadGridView
        {
            get { return this.TaskBoardRadGridView; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBoard"/> class.
        /// </summary>
        public TaskBoard()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Routes);
            this.DependentWhenVisible(VM.Clients);
            this.DependentWhenVisible(VM.Locations);

            if (DesignerProperties.IsInDesignTool)
                return;

            //Whenever the ClientsVM or LocationsVM stops loading rebind the TaskBoardRadGridView
            VM.Clients.IsLoadingObservable.Where(isLoading => !isLoading)
                .Merge(VM.Locations.IsLoadingObservable.Where(isLoading => !isLoading))
                .Subscribe(isLoading => this.TaskBoardRadGridView.Rebind());
        }
        //Logic

        private void TaskBoardRadGridViewBeginningEdit(object sender, Telerik.Windows.Controls.GridViewBeginningEditRoutedEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (!(e.Cell.ParentRow.Item is RouteTask))
            {
                e.Cancel = true;
                e.Cell.IsInEditMode = false;
            }
        }

        //Manually handle TaskBoardRadGridViewSelectionChanged for RouteTask to choose the last task for details
        private void TaskBoardRadGridViewSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            VM.TaskBoard.SelectedRouteTasks = TaskBoardRadGridView.SelectedItems.Cast<RouteTask>();
        }
    }
}

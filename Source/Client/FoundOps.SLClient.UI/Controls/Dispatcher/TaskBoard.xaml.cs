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

        #region Locals

        /// <summary>
        /// Gets the clients VM. Must be public so the xaml can bind to it.
        /// </summary>
        public ClientsVM ClientsVM
        {
            get { return VM.Clients; }
        }

        /// <summary>
        /// Gets the locations VM. Must be public so the xaml can bind to it.
        /// </summary>
        public LocationsVM LocationsVM
        {
            get { return VM.Locations; }
        }

        /// <summary>
        /// Gets the routes VM. Must be public so the xaml can bind to it.
        /// </summary>
        public RoutesVM RoutesVM
        {
            get { return VM.Routes; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBoard"/> class.
        /// </summary>
        public TaskBoard()
        {
            InitializeComponent();

            this.DependentWhenVisible(RoutesVM);
            this.DependentWhenVisible(ClientsVM);
            this.DependentWhenVisible(LocationsVM);

            if (DesignerProperties.IsInDesignTool)
                return;

            //Whenever the ClientsVM or LocationsVM stops loading rebind the TaskBoardRadGridView
            ClientsVM.IsLoadingObservable.Where(isLoading => !isLoading)
                .Merge(LocationsVM.IsLoadingObservable.Where(isLoading => !isLoading))
                .Subscribe(isLoading => this.TaskBoardRadGridView.Rebind());

            //RadDragAndDropManager.AddDragQueryHandler(TaskBoardRadGridView, OnDragQuery);
            //RadDragAndDropManager.AddDropQueryHandler(TaskBoardRadGridView, OnDropQuery);
            //RadDragAndDropManager.AddDragInfoHandler(TaskBoardRadGridView, OnDragInfo);
            //RadDragAndDropManager.AddDropInfoHandler(TaskBoardRadGridView, OnDropInfo);
        }

        //private void OnDropInfo(object sender, DragDropEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //private void OnDragInfo(object sender, DragDropEventArgs e)
        //{
        //    var draggedItems = e.Options.Source.DataContext as IEnumerable<object>;

        //    if (e.Options.Status == DragStatus.DragInProgress)
        //    {
        //        //////Set up a drag cue:
        //        //var cue = e.Options.DragCue as TreeViewDragCue;
        //        //cue.DragTooltipContent = null;
        //        //cue.DragActionContent = String.Format("Test");
        //        //cue.IsDropPossible = false;
        //        ////Here we need to choose a template for the items:
        //        //e.Options.DragCue = cue;
        //    }
        //    else if (e.Options.Status == DragStatus.DragComplete)
        //    {
        //        foreach (var draggedItem in draggedItems)
        //        {
        //            var routesVm = e.Options.Destination.DataContext as RoutesVM;

        //            if (routesVm != null) routesVm.UnroutedTasks.Remove(draggedItem as RouteTask);
        //        }
        //    }
        //}

        //private void OnDropQuery(object sender, DragDropQueryEventArgs e)
        //{
        //    e.QueryResult = false;
        //    e.Handled = true;
        //}

        //private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        //{
        //    e.QueryResult = true;
        //    e.Handled = true;
        //}

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
            VM.Routes.SelectedTaskBoardTasks = TaskBoardRadGridView.SelectedItems.Cast<RouteTask>();
        }
    }
}

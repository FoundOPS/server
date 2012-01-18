using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using Telerik.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

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
            VM.Routes.SelectedTaskBoardTasks = TaskBoardRadGridView.SelectedItems.Cast<RouteTask>();
        }
    }
}

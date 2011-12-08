using System.Collections.Generic;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    public enum DragSource
    {
        UnroutedTaskBoard,
        RotuesListBox
    }
    [ExportViewModel("RoutesDragDropVM")]
    public class RoutesDragDropVM : ThreadableVM
    {
        public DragSource OriginalDragSource { get; set; }

        public IEnumerable<RouteTask> DraggedRouteTasks { get; set; }

        public IEnumerable<RouteDestination> DraggedRouteDestinations { get; set; }

        protected override void RegisterCommands()
        {
        }

        protected override void RegisterMessages()
        {
        }
    }
}

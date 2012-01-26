using System;
using FoundOps.Common.Silverlight.UI.Tools;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Manages analytics.
    /// https://docs.google.com/a/foundops.com/spreadsheet/ccc?key=0AriWItJonH5bdGVSMTkwRUh1dUpvSjR4ZjdsdURaQnc&hl=en_US#gid=0
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Gets the current user account's name.
        /// </summary>
        public static string User
        {
            get
            {
                return Manager.Context.UserAccount.DisplayName;
            }
        }
            
            
        /// <summary>
        /// Gets the current owner account's name.
        /// </summary>
        public static string Org
        {
            get
            {
                return Manager.Context.OwnerAccount != null ? Manager.Context.OwnerAccount.DisplayName : "Unknown";
            }
        }

        //Block Icons

        /// <summary>
        /// Analytic to track which RadRibbonButton is clicked on first
        /// </summary>
        public static void FirstBlockNavigatedTo(string name)
        {
            TrackEventAction.Track("Block Icons", "FirstBlockNavigatedTo", name, 1);
            Totango.Track(Org, User, name, "FirstBlockNavigatedTo");
        }

        /// <summary>
        /// Analytic to track when any RadRibbonButton is clicked
        /// </summary>
        public static void RadRibbonButtonClick(string name)
        {
            TrackEventAction.Track("Block Icons", "RadRibbonButtonClick", name, 1);
            Totango.Track(Org, User, name, "RadRibbonButtonClick");
        }

        #region Dispatcher

        /// <summary>
        /// Analytic to track when a new route is added
        /// </summary>
        public static void AddNewRoute()
        {
            TrackEventAction.Track("Dispatcher", "AddNewRoute", 1);
            Totango.Track(Org, User, "AddNewRoute", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when a new route task is added
        /// </summary>
        public static void AddNewRouteTask()
        {
            TrackEventAction.Track("Dispatcher", "AddNewRouteTask", 1);
            Totango.Track(Org, User, "AddNewRouteTask", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when a AutoAsignJobs button is clicked
        /// </summary>
        public static void AutoAsignJobs()
        {
            TrackEventAction.Track("Dispatcher", "AutoAsignJobs", 1);
            Totango.Track(Org, User, "AutoAsignJobs", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when a route task is deleted
        /// </summary>
        public static void DeleteRouteTask()
        {
            TrackEventAction.Track("Dispatcher", "DeleteRouteTask", 1);
            Totango.Track(Org, User, "DeleteRouteTask", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when the layout changes
        /// </summary>
        public static void DispatcherLayoutChanged()
        {
            TrackEventAction.Track("Dispatcher", "LayoutChanged", 1);
            Totango.Track(Org, User, "LayoutChanged", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when the layout is reset
        /// </summary>
        public static void DispatcherLayoutReset()
        {
            TrackEventAction.Track("Dispatcher", "LayoutReset", 1);
            Totango.Track(Org, User, "LayoutReset", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when a NextDay button is clicked
        /// </summary>
        public static void NextDay()
        {
            TrackEventAction.Track("Dispatcher", "NextDay", 1);
            Totango.Track(Org, User, "GoToNextDay", "Dispatcher");
        }

        /// <summary>
        /// Analytic to track when a PreviousDay button is clicked
        /// </summary>
        public static void PreviousDay()
        {
            TrackEventAction.Track("Dispatcher", "PreviousDay", 1);
            Totango.Track(Org, User, "GoToPreviousDay", "Dispatcher");
        }

        #endregion

        #region Drag and Drop

        #region After AutoDispatch

        /// <summary>
        /// Analytic to track when dragging a task from a route to a route destination after auto dispatch
        /// </summary>
        public static void AddToRouteDestinationFromRouteAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteDestinationFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteDestinationFromRoute", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track when dragging a task from the task board to a route task after auto dispatch
        /// </summary>
        public static void AddToRouteDestinationFromTaskBoardAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteDestinationFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteDestinationFromTaskBoard", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track when dragging a task from route to another route after auto dispatch
        /// </summary>
        public static void AddToRouteFromRouteAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteFromRoute", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track dragging a task from the task board to a route after auto dispatch
        /// </summary>
        public static void AddToRouteFromTaskBoardAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteFromTaskBoard", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track when dragging a task from a route to a route task after auto dispatch
        /// </summary>
        public static void AddToRouteTaskFromRouteAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteTaskFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteTaskFromRoute", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track when dragging a task from the task board to a route task after auto dispatch
        /// </summary>
        public static void AddToRouteTaskFromTaskBoardAfterAutoDispatch(string taskName)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddToRouteTaskFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteTaskFromTaskBoard", "Drag and Drop After AutoDispatch");
        }

        /// <summary>
        /// Analytic to track when dragging a task from another route to the task board after auto dispatch
        /// </summary>
        public static void AddTaskToTaskBoardFromRouteAfterAutoDispatch(string draggedItem)
        {
            TrackEventAction.Track("Drag and Drop After AutoDispatch", "AddTaskToTaskBoardFromRoute", draggedItem, 1);
            Totango.Track(Org, User, "AddTaskToTaskBoardFromRoute", "Drag and Drop After AutoDispatch");
        }

        #endregion

        #region Before AutoDispatch

        /// <summary>
        /// Analytic to track when dragging a task from a route to a route destination
        /// </summary>
        public static void AddToRouteDestinationFromRoute(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteDestinationFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteDestinationFromRoute", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track when dragging a task from the task board to a route destination
        /// </summary>
        public static void AddToRouteDestinationFromTaskBoard(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteDestinationFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteDestinationFromTaskBoard", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track when dragging a task from route to another route
        /// </summary>
        public static void AddToRouteFromRoute(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteFromRoute", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track dragging a task from the task board to a route
        /// </summary>
        public static void AddToRouteFromTaskBoard(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteFromTaskBoard", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track when dragging a task from a route to a route task
        /// </summary>
        public static void AddToRouteTaskFromRoute(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteTaskFromRoute", taskName, 1);
            Totango.Track(Org, User, "AddToRouteTaskFromRoute", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track when dragging a task from the task board to a route task
        /// </summary>
        public static void AddToRouteTaskFromTaskBoard(string taskName)
        {
            TrackEventAction.Track("Drag and Drop", "AddToRouteTaskFromTaskBoard", taskName, 1);
            Totango.Track(Org, User, "AddToRouteTaskFromTaskBoard", "Drag and Drop");
        }

        /// <summary>
        /// Analytic to track when dragging a task from another route to the task board
        /// </summary>
        public static void AddTaskToTaskBoardFromRoute(string draggedItem)
        {
            TrackEventAction.Track("Drag and Drop", "AddTaskToTaskBoardFromRoute", draggedItem, 1);
            Totango.Track(Org, User, "AddTaskToTaskBoardFromRoute", "Drag and Drop");
        }

        #endregion

        #endregion

        //Infinite Accordion

        /// <summary>
        /// Analytic to track when the context is changed
        /// </summary>
        public static void ContextChanged(string currentContextType, string nextContextType)
        {
            TrackEventAction.Track("Infinite Accordion", "Move to Details View", String.Format("From {0} to {1}", currentContextType, nextContextType), 1);
            Totango.Track(Org, User, String.Format("Move to Details View from {0} to {1}", currentContextType, nextContextType), "Infinite Accordion");
        }

        /// <summary>
        /// Analytic to track the context depth when the context is changed
        /// </summary>
        public static void ContextDepth(int depth)
        {
            TrackEventAction.Track("Infinite Accordion", "Context Depth", depth);
            Totango.Track(Org, User, string.Format("Context Depth - {0}", depth), "Infinite Accordion");
        }

        #region Route Manifests

        /// <summary>
        /// Analytic to track when the Address option is selected
        /// </summary>
        public static void Address()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Address", 1);
            Totango.Track(Org, User, "Address", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the AssignedVehicles option is selected
        /// </summary>
        public static void AssignedVehicles()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "AssignedVehicles", 1);
            Totango.Track(Org, User, "AssignedVehicles", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the 2DBarcode option is selected
        /// </summary>
        public static void Barcode()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "2DBarcode", 1);
            Totango.Track(Org, User, "2DBarcode", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the ContactInfo option is selected
        /// </summary>
        public static void ContactInfo()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "ContactInfo", 1);
            Totango.Track(Org, User, "ContactInfo", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the CustomMessage option is selected
        /// </summary>
        public static void CustomMessage()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "CustomMessage", 1);
            Totango.Track(Org, User, "CustomMessage", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the Destinations option is selected
        /// </summary>
        public static void Destinations()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Destinations", 1);
            Totango.Track(Org, User, "Destinations", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the DestinationsSummary option is selected
        /// </summary>
        public static void DestinationsSummary()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "DestinationsSummary", 1);
            Totango.Track(Org, User, "DestinationsSummary", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the EndTime option is selected
        /// </summary>
        public static void EndTime()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "EndTime", 1);
            Totango.Track(Org, User, "EndTime", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the Footer option is selected
        /// </summary>
        public static void Footer()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Footer", 1);
            Totango.Track(Org, User, "Footer", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the Header option is selected
        /// </summary>
        public static void Header()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Header", 1);
            Totango.Track(Org, User, "Header", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the NumberofDestinations option is selected
        /// </summary>
        public static void NumberofDestinations()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "NumberofDestinations", 1);
            Totango.Track(Org, User, "NumberofDestinations", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the NumberOfTasks option is selected
        /// </summary>
        public static void NumberOfTasks()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "NumberOfTasks", 1);
            Totango.Track(Org, User, "NumberOfTasks", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the PageNumbers option is selected
        /// </summary>
        public static void PageNumbers()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "PageNumbers", 1);
            Totango.Track(Org, User, "PageNumbers", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the RouteDate option is selected
        /// </summary>
        public static void RouteDate()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteDate", 1);
            Totango.Track(Org, User, "RouteDate", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the route manifests are printed
        /// </summary>
        public static void RouteManifestPrinted(string time)
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestPrinted", "Time: " + time, 1);
            Totango.Track(Org, User, "Time: " + time, "RouteManifestPrinted");
        }

        /// <summary>
        /// Analytic to track when the RouteName option is selected
        /// </summary>
        public static void RouteName()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteName", 1);
            Totango.Track(Org, User, "RouteName", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the RouteSummary option is selected
        /// </summary>
        public static void RouteSummary()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteSummary", 1);
            Totango.Track(Org, User, "RouteSummary", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the RouteTasks option is selected
        /// </summary>
        public static void RouteTasks()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteTasks", 1);
            Totango.Track(Org, User, "RouteTasks", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the StartTime option is selected
        /// </summary>
        public static void StartTime()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "StartTime", 1);
            Totango.Track(Org, User, "StartTime", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the Summary option is selected
        /// </summary>
        public static void Summary()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Summary", 1);
            Totango.Track(Org, User, "Summary", "RouteManifestOptionSelected");
        }

        /// <summary>
        /// Analytic to track when the TaskSummary option is selected
        /// </summary>
        public static void TaskSummary()
        {
            TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "TaskSummary", 1);
            Totango.Track(Org, User, "TaskSummary", "RouteManifestOptionSelected");
        }

        #endregion
    }
}
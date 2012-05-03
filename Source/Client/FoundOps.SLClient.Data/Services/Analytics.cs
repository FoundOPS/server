using System;
using FoundOps.Common.Silverlight.UI.Tools;
using System.Windows.Browser;
using System.Collections.Generic;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// This is the Activity for Totango and Action for Google Analytics.
    /// </summary>
    public enum Event
    {
        //DragDestinationToTaskBoard,
        //DragTaskToTaskBoard,
        //DragDestinationFromRouteToRoute,
        //DragTaskFromRouteToRoute,
        //DragDestinationFromTaskBoardToRoute,
        //DragTaskFromTaskBoardToRoute,
        AddRoute,
        AutoAssignJobs,
        //Not part of a specific section
        ContextChanged,
        DeleteRouteTask,
        DispatcherLayoutChanged,
        DispatcherLayoutReset,
        DispatcherChangeDate,
        DispatcherNextDay,
        DispatcherPreviousDay,
        FirstSectionChosen,
        //Only tracked on Google Analytics
        ManifestOption,
        PrintedManifest,
        SectionChosen
    }

    /// <summary>
    /// This is Module for Totango and Category for Google Analytics.
    /// </summary>
    public enum Section
    {
        BusinessAccounts,
        Clients,
        Contacts,
        Dispatcher,
        Employees,
        FeedbackAndSupport,
        Locations,
        Logout,
        None,
        Regions,
        Services,
        Settings,
        Vehicles
    }

    /// <summary>
    /// Manages analytics.
    /// https://docs.google.com/a/foundops.com/spreadsheet/ccc?key=0AriWItJonH5bdGVSMTkwRUh1dUpvSjR4ZjdsdURaQnc&hl=en_US#gid=0
    /// </summary>
    public static class Analytics
    {
        #region Constants

#if RELEASE
        //This is the Production Totango Service Id
        private const string TotangoServiceId = "SP-1268-01";
#else //if DEBUG or TESTRELEASE
        //The ServiceId for the Totango account
        //This is the Development Totango Service Id
        private const string TotangoServiceId = "SP-12680-01";
#endif

        /// <summary>
        /// Holds the string value of the name for each event
        /// </summary>
        public static Dictionary<Event, String> EventNames = new Dictionary<Event, String>();

        /// <summary>
        /// Holds the sections for each event
        /// </summary>
        public static Dictionary<Event, Section> EventSections = new Dictionary<Event, Section>();

        /// <summary>
        /// Holds the string value of the name for each section
        /// </summary>
        public static Dictionary<Section, String> SectionNames = new Dictionary<Section, String>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="Analytics"/> class.
        /// </summary>
        static Analytics()
        {
            //Set a section to all events that have a specific section
            EventSections.Add(Event.AddRoute, Section.Dispatcher);
            EventSections.Add(Event.AutoAssignJobs, Section.Dispatcher);
            EventSections.Add(Event.DeleteRouteTask, Section.Dispatcher);
            EventSections.Add(Event.DispatcherLayoutChanged, Section.Dispatcher);
            EventSections.Add(Event.DispatcherLayoutReset, Section.Dispatcher);
            EventSections.Add(Event.DispatcherChangeDate, Section.Dispatcher);
            EventSections.Add(Event.DispatcherNextDay, Section.Dispatcher);
            EventSections.Add(Event.DispatcherPreviousDay, Section.Dispatcher);
            EventSections.Add(Event.PrintedManifest, Section.Dispatcher);

            //Set the string value of each Section's name
            SectionNames.Add(Section.BusinessAccounts, "Business Accounts");
            SectionNames.Add(Section.Contacts, "Contacts");
            SectionNames.Add(Section.Clients, "Clients");
            SectionNames.Add(Section.Dispatcher, "Dispatcher");
            SectionNames.Add(Section.Employees, "Employees");
            SectionNames.Add(Section.FeedbackAndSupport, "Feedback and Support");
            SectionNames.Add(Section.Locations, "Locations");
            SectionNames.Add(Section.Logout, "Logout");
            SectionNames.Add(Section.None, "None");
            SectionNames.Add(Section.Regions, "Regions");
            SectionNames.Add(Section.Services, "Services");
            SectionNames.Add(Section.Settings, "Settings");
            SectionNames.Add(Section.Vehicles, "Vehicles");

            //Set the string value of each Event's name
            EventNames.Add(Event.FirstSectionChosen, "First Section Chosen");
            EventNames.Add(Event.SectionChosen, "Section Chosen");
            EventNames.Add(Event.AddRoute, "Add Route");
            EventNames.Add(Event.AutoAssignJobs, "Auto-Assign Jobs");
            EventNames.Add(Event.DeleteRouteTask, "Delete Route Task");
            EventNames.Add(Event.DispatcherLayoutChanged, "Layout Changed");
            EventNames.Add(Event.DispatcherLayoutReset, "Layout Reset");
            EventNames.Add(Event.DispatcherChangeDate, "Change Date");
            EventNames.Add(Event.DispatcherNextDay, "Next Day");
            EventNames.Add(Event.DispatcherPreviousDay, "Previous Day");
            EventNames.Add(Event.PrintedManifest, "Printed Manifest");
            EventNames.Add(Event.ManifestOption, "Manifest Option");
            EventNames.Add(Event.ContextChanged, "Context Changed");
        }

        #endregion

        #region Logic

        /// <summary>
        /// Tracks the event in Totango and Google Analytics
        /// </summary>
        /// <param name="trackedEvent">The tracked event.</param>
        /// <param name="section">The section. Only need to specify this if it is not obvious (part of _eventSections in Analytics).</param>
        /// <param name="detail">A detailed piece of information about the event (only for Google Analytics).</param>
        public static void Track(Event trackedEvent, Section? section = null, string detail = null)
        {
            //If the section is not specified, try to get it from the event sections dictionary
            if (!section.HasValue)
                section = EventSections.ContainsKey(trackedEvent) ? EventSections[trackedEvent] : Section.None;

            //Track the event
            TrackHelper(section, trackedEvent, detail);
        }

        /// <summary>
        /// Tracks the event in Totango and Google Analytics
        /// </summary>
        /// <param name="section">The section of the event.</param>
        /// <param name="trackedEvent">The event to track.</param>
        /// <param name="detail">A detailed piece of information about the event (only for Google Analytics).</param>
        private static void TrackHelper(Section? section, Event trackedEvent, string detail)
        {
            var currentOrganization = Manager.Context.OwnerAccount != null ? Manager.Context.OwnerAccount.DisplayName : "Unknown";
            var currentUser = Manager.Context.UserAccount.DisplayName;

            //Do not track FoundOPS users
            var email = Manager.Context.UserAccount.EmailAddress ?? "";
            if (email.Contains("foundops.com"))
                return;

            if (section == null)
                section = Section.None;

            //Create Totango action string
            var url = string.Format("http://sdr.totango.com/pixel.gif/?sdr_s={0}&sdr_o={1}&sdr_u={2}&sdr_a={3}&sdr_m={4}", TotangoServiceId,
                    HttpUtility.UrlEncode(currentOrganization), HttpUtility.UrlEncode(currentUser), HttpUtility.UrlEncode(trackedEvent.ToString()), HttpUtility.UrlEncode(section.ToString()));

            //Call Totango API (for every event except Manifest Option)
            if (trackedEvent != Event.ManifestOption)
                HtmlPage.Window.Invoke("httpGet", new object[] { url });

            //Call Google Analytics API
            HtmlPage.Window.Invoke("trackEvent", new object[] { section, trackedEvent, detail });
        }

        #endregion
    }
}
using System.ComponentModel;

namespace FoundOps.SLClient.Data.Models
{
    /// <summary>
    /// Holds the RouteManifestSettings
    /// </summary>
    // ReSharper disable CSharpWarnings::CS1591
    public class RouteManifestSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Public Properties

        #region Header

        #region Master Header Control

        private bool _isHeaderVisible;
        public bool IsHeaderVisible
        {
            get { return _isHeaderVisible; }
            set
            {
                _isHeaderVisible = value;
                this.RaisePropertyChanged("IsHeaderVisible");
            }
        }

        #endregion

        #region Route Name

        private bool _isRouteNameVisible;
        public bool IsRouteNameVisible
        {
            get { return _isRouteNameVisible; }
            set
            {
                _isRouteNameVisible = value;
                this.RaisePropertyChanged("IsRouteNameVisible");
            }
        }

        #endregion

        #region Route Date

        private bool _isRouteDateVisible;
        public bool IsRouteDateVisible
        {
            get { return _isRouteDateVisible; }
            set
            {
                _isRouteDateVisible = value;
                this.RaisePropertyChanged("IsRouteDateVisible");
            }
        }

        #endregion

        #region Assigned Vehicles

        private bool _isAssignedVehiclesVisible;
        public bool IsAssignedVehiclesVisible
        {
            get { return _isAssignedVehiclesVisible; }
            set
            {
                _isAssignedVehiclesVisible = value;
                this.RaisePropertyChanged("IsAssignedVehiclesVisible");
            }
        }

        #endregion

        #region Assigned Technicians

        private bool _isAssignedTechniciansVisible;
        public bool IsAssignedTechniciansVisible
        {
            get { return _isAssignedTechniciansVisible; }
            set
            {
                _isAssignedTechniciansVisible = value;
                this.RaisePropertyChanged("IsAssignedTechniciansVisible");
            }
        }

        #endregion

        #endregion

        #region Summary

        #region Summary Master

        private bool _isSummaryVisible;
        public bool IsSummaryVisible
        {
            get { return _isSummaryVisible; }
            set
            {
                _isSummaryVisible = value;
                this.RaisePropertyChanged("IsSummaryVisible");
            }
        }

        #endregion

        #region Route Map

        private bool _isRouteMapVisible;
        public bool IsRouteMapVisible
        {
            get { return _isRouteMapVisible; }
            set
            {
                _isRouteMapVisible = value;
                this.RaisePropertyChanged("IsRouteMapVisible");
            }
        }

        #endregion


        #region Route Summary

        #region Route Summary Master

        private bool _isRouteSummaryVisible;
        public bool IsRouteSummaryVisible
        {
            get { return _isRouteSummaryVisible; }
            set
            {
                _isRouteSummaryVisible = value;
                this.RaisePropertyChanged("IsRouteSummaryVisible");
            }
        }

        #endregion

        #region Capabilities

        private bool _isCapabilitiesVisible;
        public bool IsCapabilitiesVisible
        {
            get { return _isCapabilitiesVisible; }
            set
            {
                _isCapabilitiesVisible = value;
                this.RaisePropertyChanged("IsCapabilitiesVisible");
            }
        }

        #endregion

        #region Scheduled Start Time

        private bool _isScheduledStartTimeVisible;
        public bool IsScheduledStartTimeVisible
        {
            get { return _isScheduledStartTimeVisible; }
            set
            {
                _isScheduledStartTimeVisible = value;
                this.RaisePropertyChanged("IsScheduledStartTimeVisible");
            }
        }

        #endregion

        #region Scheduled End Time

        private bool _isScheduledEndTimeVisible;
        public bool IsScheduledEndTimeVisible
        {
            get { return _isScheduledEndTimeVisible; }
            set
            {
                _isScheduledEndTimeVisible = value;
                this.RaisePropertyChanged("IsScheduledEndTimeVisible");
            }
        }

        #endregion

        #region Estimated End Time

        private bool _isEstimatedEndTimeVisible;
        public bool IsEstimatedEndTimeVisible
        {
            get { return _isEstimatedEndTimeVisible; }
            set
            {
                _isEstimatedEndTimeVisible = value;
                this.RaisePropertyChanged("IsEstimatedEndTimeVisible");
            }
        }

        #endregion

        #region Scheduled Duratoin

        private bool _isScheduledDurationVisible;
        public bool IsScheduledDurationVisible
        {
            get { return _isScheduledDurationVisible; }
            set
            {
                _isScheduledDurationVisible = value;
                this.RaisePropertyChanged("IsScheduledDurationVisible");
            }
        }

        #endregion

        #region Estimated Duration

        private bool _isEstimatedDurationVisible;
        public bool IsEstimatedDurationVisible
        {
            get { return _isEstimatedDurationVisible; }
            set
            {
                _isEstimatedDurationVisible = value;
                this.RaisePropertyChanged("IsEstimatedDurationVisible");
            }
        }

        #endregion

        #region Total Distance

        private bool _isTotalDistanceVisible;
        public bool IsTotalDistanceVisible
        {
            get { return _isTotalDistanceVisible; }
            set
            {
                _isTotalDistanceVisible = value;
                this.RaisePropertyChanged("IsTotalDistanceVisible");
            }
        }

        #endregion

        #endregion

        #region Destinations Summary

        #region Destinations Summary Master

        private bool _isDestinationsSummaryVisible;
        public bool IsDestinationsSummaryVisible
        {
            get { return _isDestinationsSummaryVisible; }
            set
            {
                _isDestinationsSummaryVisible = value;
                this.RaisePropertyChanged("IsDestinationsSummaryVisible");
            }
        }

        #endregion

        #region Number Of Destinations

        private bool _isNumberofDestinationsVisible;
        public bool IsNumberofDestinationsVisible
        {
            get { return _isNumberofDestinationsVisible; }
            set
            {
                _isNumberofDestinationsVisible = value;
                this.RaisePropertyChanged("IsNumberofDestinationsVisible");
            }
        }

        #endregion

        #region Average Time At Destinations

        private bool _isAverageTimeAtDestinationVisible;
        public bool IsAverageTimeAtDestinationVisible
        {
            get { return _isAverageTimeAtDestinationVisible; }
            set
            {
                _isAverageTimeAtDestinationVisible = value;
                this.RaisePropertyChanged("IsAverageTimeAtDestinationVisible");
            }
        }

        #endregion

        #region Average Time Between Destinations

        private bool _isAverageTimeBetweenDestinationsVisible;
        public bool IsAverageTimeBetweenDestinationsVisible
        {
            get { return _isAverageTimeBetweenDestinationsVisible; }
            set
            {
                _isAverageTimeBetweenDestinationsVisible = value;
                this.RaisePropertyChanged("IsAverageTimeBetweenDestinationsVisible");
            }
        }

        #endregion

        #region Average Distance Between Destinations

        private bool _isAverageDistanceBetweenDestinationsVisible;
        public bool IsAverageDistanceBetweenDestinationsVisible
        {
            get { return _isAverageDistanceBetweenDestinationsVisible; }
            set
            {
                _isAverageDistanceBetweenDestinationsVisible = value;
                this.RaisePropertyChanged("IsAverageDistanceBetweenDestinationsVisible");
            }
        }

        #endregion

        #endregion

        #region Task Summary

        #region Task Summary Master

        private bool _isTaskSummaryVisible;
        public bool IsTaskSummaryVisible
        {
            get { return _isTaskSummaryVisible; }
            set
            {
                _isTaskSummaryVisible = value;
                this.RaisePropertyChanged("IsTaskSummaryVisible");
            }
        }

        #endregion

        #region Number Of Tasks

        private bool _isNumberOfTasksVisible;
        public bool IsNumberOfTasksVisible
        {
            get { return _isNumberOfTasksVisible; }
            set
            {
                _isNumberOfTasksVisible = value;
                this.RaisePropertyChanged("IsNumberOfTasksVisible");
            }
        }

        #endregion

        #region Average Task Duration

        private bool _isAverageTaskDurationVisible;
        public bool IsAverageTaskDurationVisible
        {
            get { return _isAverageTaskDurationVisible; }
            set
            {
                _isAverageTaskDurationVisible = value;
                this.RaisePropertyChanged("IsAverageTaskDurationVisible");
            }
        }

        #endregion

        #region Task Breakdown

        private bool _isTaskBreakdownVisible;
        public bool IsTaskBreakdownVisible
        {
            get { return _isTaskBreakdownVisible; }
            set
            {
                _isTaskBreakdownVisible = value;
                this.RaisePropertyChanged("IsTaskBreakdownVisible");
            }
        }

        #endregion

        #endregion

        #endregion

        #region Destinations

        //All the properties which affect destinations
        public static string[] DestinationsProperties = new[]
                                                     {
                                                         "IsDestinationsVisible", "IsAddressVisible",
                                                         "IsContactInfoVisible", "IsPhoneNumberVisible",
                                                         "IsFaxNumberVisible", "IsEmailAddressVisible",
                                                         "IsWebsiteVisible", "IsOtherContactInfoVisible",
                                                         "IsRouteTasksVisible","IsFieldsVisible", "IsTaskSpecificDetailsVisible",
                                                         "IsNotesVisible", "IsMapVisible",
                                                         "IsSubLocationsVisible", "Is2DBarcodeVisible",
                                                         "IsUserUploadedImageVisible"
                                                     };

        #region Destinations Master

        private bool _isDestinationsVisible;
        public bool IsDestinationsVisible
        {
            get { return _isDestinationsVisible; }
            set
            {
                _isDestinationsVisible = value;
                this.RaisePropertyChanged("IsDestinationsVisible");
            }
        }

        #endregion

        #region Address

        private bool _isAddressVisible;
        public bool IsAddressVisible
        {
            get { return _isAddressVisible; }
            set
            {
                _isAddressVisible = value;
                this.RaisePropertyChanged("IsAddressVisible");
            }
        }

        #endregion

        #region Contact Info

        #region Contact Info Master

        private bool _isContactInfoVisible;
        public bool IsContactInfoVisible
        {
            get { return _isContactInfoVisible; }
            set
            {
                _isContactInfoVisible = value;
                this.RaisePropertyChanged("IsContactInfoVisible");
            }
        }

        #endregion

        #region Phone Number

        private bool _isPhoneNumberVisible;
        public bool IsPhoneNumberVisible
        {
            get { return _isPhoneNumberVisible; }
            set
            {
                _isPhoneNumberVisible = value;
                this.RaisePropertyChanged("IsPhoneNumberVisible");
            }
        }

        #endregion

        #region Fax Number

        private bool _isFaxNumberVisible;
        public bool IsFaxNumberVisible
        {
            get { return _isFaxNumberVisible; }
            set
            {
                _isFaxNumberVisible = value;
                this.RaisePropertyChanged("IsFaxNumberVisible");
            }
        }

        #endregion

        #region Email Address

        private bool _isEmailAddressVisible;
        public bool IsEmailAddressVisible
        {
            get { return _isEmailAddressVisible; }
            set
            {
                _isEmailAddressVisible = value;
                this.RaisePropertyChanged("IsEmailAddressVisible");
            }
        }

        #endregion

        #region Website

        private bool _isWebsiteVisible;
        public bool IsWebsiteVisible
        {
            get { return _isWebsiteVisible; }
            set
            {
                _isWebsiteVisible = value;
                this.RaisePropertyChanged("IsWebsiteVisible");
            }
        }

        #endregion

        #region Other Contact Info

        private bool _isOtherContactInfoVisible;
        public bool IsOtherContactInfoVisible
        {
            get { return _isOtherContactInfoVisible; }
            set
            {
                _isOtherContactInfoVisible = value;
                this.RaisePropertyChanged("IsOtherContactInfoVisible");
            }
        }

        #endregion

        #endregion

        #region Route Tasks

        #region Route Tasks Master

        private bool _isRouteTasksVisible;
        public bool IsRouteTasksVisible
        {
            get { return _isRouteTasksVisible; }
            set
            {
                _isRouteTasksVisible = value;
                this.RaisePropertyChanged("IsRouteTasksVisible");
            }
        }

        #endregion

        #region Fields

        private bool _isFieldsVisible;
        public bool IsFieldsVisible
        {
            get { return _isFieldsVisible; }
            set
            {
                _isFieldsVisible = value;
                this.RaisePropertyChanged("IsFieldsVisible");
            }
        }

        #endregion

        #region Task Specific Details

        private bool _isTaskSpecificDetailsVisible;
        public bool IsTaskSpecificDetailsVisible
        {
            get { return _isTaskSpecificDetailsVisible; }
            set
            {
                _isTaskSpecificDetailsVisible = value;
                this.RaisePropertyChanged("IsTaskSpecificDetailsVisible");
            }
        }

        #endregion

        #region Notes

        private bool _isNotesVisible;
        public bool IsNotesVisible
        {
            get { return _isNotesVisible; }
            set
            {
                _isNotesVisible = value;
                this.RaisePropertyChanged("IsNotesVisible");
            }
        }

        #endregion

        #endregion

        #region Map

        private bool _isMapVisible;
        public bool IsMapVisible
        {
            get { return _isMapVisible; }
            set
            {
                _isMapVisible = value;
                this.RaisePropertyChanged("IsMapVisible");
            }
        }

        #endregion

        #region SubLocations

        private bool _isSubLocationsVisible;
        public bool IsSubLocationsVisible
        {
            get { return _isSubLocationsVisible; }
            set
            {
                _isSubLocationsVisible = value;
                this.RaisePropertyChanged("IsSubLocationsVisible");
            }
        }

        #endregion

        #region 2D Barcode

        private bool _is2DBarcodeVisible;
        public bool Is2DBarcodeVisible
        {
            get { return _is2DBarcodeVisible; }
            set
            {
                _is2DBarcodeVisible = value;
                this.RaisePropertyChanged("Is2DBarcodeVisible");
            }
        }

        #endregion

        #region User Uploaded Image

        private bool _isUserUploadedImageVisible;
        public bool IsUserUploadedImageVisible
        {
            get { return _isUserUploadedImageVisible; }
            set
            {
                _isUserUploadedImageVisible = value;
                this.RaisePropertyChanged("IsUserUploadedImageVisible");
            }
        }

        #endregion

        #endregion

        #region Transit Between Destinations

        #region Transit Between Destinations Master

        private bool _isTransitBetweenDestinationsVisible;
        public bool IsTransitBetweenDestinationsVisible
        {
            get { return _isTransitBetweenDestinationsVisible; }
            set
            {
                _isTransitBetweenDestinationsVisible = value;
                this.RaisePropertyChanged("IsTransitBetweenDestinationsVisible");
            }
        }

        #endregion

        #region Estimated Distance

        private bool _isEstimatedDistanceVisible;
        public bool IsEstimatedDistanceVisible
        {
            get { return _isEstimatedDistanceVisible; }
            set
            {
                _isEstimatedDistanceVisible = value;
                this.RaisePropertyChanged("IsEstimatedDistanceVisible");
            }
        }

        #endregion

        #region Estimated Time

        private bool _isEstimatedTimeVisible;
        public bool IsEstimatedTimeVisible
        {
            get { return _isEstimatedTimeVisible; }
            set
            {
                _isEstimatedTimeVisible = value;
                this.RaisePropertyChanged("IsEstimatedTimeVisible");
            }
        }

        #endregion

        #endregion

        #region Footer

        #region Footer Master

        private bool _isFooterVisible;
        public bool IsFooterVisible
        {
            get { return _isFooterVisible; }
            set
            {
                _isFooterVisible = value;
                this.RaisePropertyChanged("IsFooterVisible");
            }
        }

        #endregion

        #region Page Numbers

        private bool _isPageNumbersVisible;
        public bool IsPageNumbersVisible
        {
            get { return _isPageNumbersVisible; }
            set
            {
                _isPageNumbersVisible = value;
                this.RaisePropertyChanged("IsPageNumbersVisible");
            }
        }

        #endregion

        #region Custom Message

        private bool _isCustomMessageVisible;
        public bool IsCustomMessageVisible
        {
            get { return _isCustomMessageVisible; }
            set
            {
                _isCustomMessageVisible = value;
                this.RaisePropertyChanged("IsCustomMessageVisible");
            }
        }

        private string _customMessage;
        public string CustomMessage
        {
            get { return _customMessage; }
            set
            {
                _customMessage = value;
                this.RaisePropertyChanged("CustomMessage");
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestSettings"/> class.
        /// </summary>
        public RouteManifestSettings()
        {
            #region Header

            IsHeaderVisible = true;
            IsRouteNameVisible = true;
            IsRouteDateVisible = true;
            IsAssignedVehiclesVisible = true;
            IsAssignedTechniciansVisible = false;

            #endregion

            #region Summary

            IsSummaryVisible = true;
            //IsRouteMapVisible = true;

            #region Route Summary

            IsRouteSummaryVisible = true;

            //IsCapabilitiesVisible = true;
            IsScheduledStartTimeVisible = true;
            IsScheduledEndTimeVisible = true;
            IsEstimatedEndTimeVisible = false;
            IsScheduledDurationVisible = false;
            IsEstimatedDurationVisible = false;
            IsTotalDistanceVisible = false;

            #endregion

            #region Destinations Summary

            IsDestinationsSummaryVisible = true;
            IsNumberofDestinationsVisible = true;
            IsAverageTimeAtDestinationVisible = false;
            IsAverageTimeBetweenDestinationsVisible = false;
            IsAverageDistanceBetweenDestinationsVisible = false;

            #endregion

            #region Task Summary

            IsTaskSummaryVisible = true;
            IsNumberOfTasksVisible = true;
            IsAverageTaskDurationVisible = false;
            IsTaskBreakdownVisible = false;

            #endregion

            #endregion

            #region Destinations

            IsDestinationsVisible = true;
            IsAddressVisible = true;

            #region Contact Info

            IsContactInfoVisible = true;
            //IsPhoneNumberVisible = true;
            IsFaxNumberVisible = false;
            IsEmailAddressVisible = false;
            IsWebsiteVisible = false;
            //IsOtherContactInfoVisible = true;

            #endregion

            #region Route Tasks

            IsRouteTasksVisible = true;
            //IsTaskSpecificDetailsVisible = true;
            //IsNotesVisible = true;
            IsFieldsVisible = true;
            #endregion

            //IsMapVisible = false;
            //IsSubLocationsVisible = false;
            Is2DBarcodeVisible = true;
            IsUserUploadedImageVisible = false;

            #endregion

            #region TransitBetweenDestinations

            IsTransitBetweenDestinationsVisible = true;
            IsEstimatedDistanceVisible = true;
            IsEstimatedTimeVisible = true;

            #endregion

            #region Footer

            IsFooterVisible = true;
            IsPageNumbersVisible = true;
            IsCustomMessageVisible = true;

            CustomMessage = "Drive safely and thank you for your hard work";

            #endregion
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    // ReSharper restore CSharpWarnings::CS1591
}

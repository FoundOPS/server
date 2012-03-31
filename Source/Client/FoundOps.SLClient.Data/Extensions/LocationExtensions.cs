using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.UI.Interfaces;
using ReactiveUI;
using System;
using System.ComponentModel;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Location : IRaiseValidationErrors, IReactiveNotifyPropertyChanged, ILoadDetails
    {
        #region Public Properties

        #region Implementation of ILoadDetails

        private bool _detailsLoaded;
        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        /// <summary>
        /// Gets the telerik location.
        /// </summary>
        public Telerik.Windows.Controls.Map.Location? TelerikLocation
        {
            get
            {
                if (this.Latitude == null || this.Longitude == null)
                    return null;

                return new Telerik.Windows.Controls.Map.Location(System.Convert.ToDouble(this.Latitude),
                                                                 System.Convert.ToDouble(this.Longitude));
            }
        }

        private OrderedEntityCollection<SubLocation> _subLocationsListWrapper;
        /// <summary>
        /// Gets the sub locations list wrapper. It orders and numbers the SubLocations automatically when items are added and removed.
        /// </summary>
        public OrderedEntityCollection<SubLocation> SubLocationsListWrapper
        {
            get { return _subLocationsListWrapper; }
            private set
            {
                _subLocationsListWrapper = value;
                this.RaisePropertyChanged("SubLocationsListWrapper");
            }
        }

        #region IReactiveNotifyPropertyChanged

        public event PropertyChangingEventHandler PropertyChanging;

        MakeObjectReactiveHelper _reactiveHelper;

        public IObservable<IObservedChange<object, object>> Changed { get { return _reactiveHelper.Changed; } }

        public IObservable<IObservedChange<object, object>> Changing { get { return _reactiveHelper.Changing; } }

        public IDisposable SuppressChangeNotifications()
        {
            return _reactiveHelper.SuppressChangeNotifications();
        }

        #endregion

        #endregion

        #region Initialization

        partial void OnCreation()
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            if (isInitialLoad)
                SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the validation errors.
        /// </summary>
        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }

        #endregion
    }
}


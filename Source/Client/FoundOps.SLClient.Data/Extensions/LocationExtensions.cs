using System;
using ReactiveUI;
using System.ComponentModel;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.Interfaces;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Location : IRaiseValidationErrors, IReactiveNotifyPropertyChanged
    {
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

        private bool _detailsLoading;

        /// <summary>
        /// Gets or sets a value indicating whether [details loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoading
        {
            get { return _detailsLoading; }
            set
            {
                _detailsLoading = value;
                this.RaisePropertyChanged("DetailsLoading");
            }
        }

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

        /// <summary>
        /// Raises the validation errors.
        /// </summary>
        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }
    }
}


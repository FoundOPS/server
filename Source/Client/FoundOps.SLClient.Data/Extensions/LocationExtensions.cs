﻿using System.IO;
using System.Net;
using System.Reactive.Linq;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.Models.Collections;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Converters;
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
                //Cannot clear details loaded. This is prevent issues when saving.
                if (_detailsLoaded)
                    return;

                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        #region Barcode

        private bool _barcodeLoading;
        /// <summary>
        /// True when the barcode is loading.
        /// </summary>
        public bool BarcodeLoading
        {
            get { return _barcodeLoading; }
            private set
            {
                _barcodeLoading = value;
                RaisePropertyChanged("BarcodeLoading");
            }
        }

        private byte[] _barcodeImage;
        /// <summary>
        /// The barcode image.
        /// </summary>
        public byte[] BarcodeImage
        {
            get { return _barcodeImage; }
            private set
            {
                _barcodeImage = value;
                this.RaisePropertyChanged("BarcodeImage");
            }
        }

        /// <summary>
        /// Contains the loaded barcode image's latitude and longitude
        /// </summary>
        private Tuple<decimal?, decimal?> _barcodeLatitudeLongitudeTuple = null;

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

                return new Telerik.Windows.Controls.Map.Location(Convert.ToDouble(this.Latitude), Convert.ToDouble(this.Longitude));
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

        private Func<SubLocation, OrderedEntityCollection<SubLocation>> GetSubLocationsListWrapper
        {
            get
            {
                return subLocation => subLocation.Location == null ? null : subLocation.Location.SubLocationsListWrapper;
            }
        }

        partial void OnCreation()
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false, GetSubLocationsListWrapper);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            if (isInitialLoad)
                SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false, GetSubLocationsListWrapper);
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

        /// <summary>
        /// A method to update the barcode if it is not loaded.
        /// </summary>
        public void UpdateBarcode()
        {
            //if the barcode is currently loading return
            if (BarcodeLoading)
                return;

            //if the barcode image is already loaded return
            if (BarcodeImage != null && _barcodeLatitudeLongitudeTuple != null &&
                _barcodeLatitudeLongitudeTuple.Item1 == this.Latitude && _barcodeLatitudeLongitudeTuple.Item2 == this.Longitude)
                return;

            //if the lat and lon are null, set the image to null
            if (!this.Latitude.HasValue || !this.Longitude.HasValue)
            {
                BarcodeImage = null;
                BarcodeLoading = false;
                return;
            }

            //Load the barcode

            BarcodeLoading = true;

            var urlConverter = new LocationToUrlConverter();
            var client = new WebClient();
            client.OpenReadObservable((Uri)urlConverter.Convert(this, null, null, null))
                //If there is an error return a null stream
                .ObserveOnDispatcher().Subscribe(stream =>
                {
                    this.BarcodeImage = stream != null ? stream.ReadFully() : null;
                    _barcodeLatitudeLongitudeTuple = new Tuple<decimal?, decimal?>(this.Latitude, this.Longitude);
                    BarcodeLoading = false;
                });

            //If loading the barcode takes more than 10 seconds, set BarcodeLoading = false, and the BarcodeImage = null
            Rxx3.RunDelayed(TimeSpan.FromSeconds(10), () =>
            {
                if (!BarcodeLoading) return;
                BarcodeImage = null;
                BarcodeLoading = false;
            });
        }

    }
}


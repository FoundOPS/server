using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RecurringService : ILoadDetails
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a Service based off this ServiceTemplate for the specified date.
        /// </summary>
        /// <param name="date">The date to generate a service for.</param>
        public Service GenerateServiceOnDate(DateTime date)
        {
            return new Service
            {
                Generated =  true,
                ServiceDate = date,
                ClientId = this.ClientId,
                Client = this.Client,
                RecurringServiceParent = this,
                ServiceProviderId = this.Client.VendorId,
                ServiceProvider = this.Client.Vendor,
                ServiceTemplate = this.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined)
            };
        }

        /// <summary>
        /// An observable that publishes this Repeat's PropertyChangedEventArgs.
        /// </summary>
        /// <returns></returns>
        public IObservable<PropertyChangedEventArgs> RepeatChangedObservable()
        {
            return this.Repeat == null ? 
                Observable.Empty<PropertyChangedEventArgs>() 
                : this.Repeat.FromAnyPropertyChanged();
        }

        #endregion
    }
}
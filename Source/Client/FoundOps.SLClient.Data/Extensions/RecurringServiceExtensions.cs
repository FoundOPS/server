using FoundOps.Common.Tools;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RecurringService
    {
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
    }
}
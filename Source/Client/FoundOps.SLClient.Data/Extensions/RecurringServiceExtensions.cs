using System;
using System.Reactive.Linq;
using System.ComponentModel;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class RecurringService
    {
        public Service GenerateServiceOnDate(DateTime date)
        {
            return new Service
            {
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

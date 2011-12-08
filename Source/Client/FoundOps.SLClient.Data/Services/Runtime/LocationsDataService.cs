using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Context.Services.Interface;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;
using ScalableCourier.Client.CommonResources.MVVM.Services;

namespace FoundOps.Core.Context.Services
{
    [ExportService(ServiceType.Runtime, typeof(ILocationsDataService))]
    public class LocationsDataService : DomainContextDataService<CoreDomainContext>, ILocationsDataService
    {
        [ImportingConstructor]
        public LocationsDataService(CoreDomainContext context)
            : base(context)
        {
        }

        public void GetLocations(Guid roleId, Action<ObservableCollection<Location>> getLocationsCallback)
        {
            LoadCollectionEntityList(Context.GetLocationsToAdministerForRoleQuery(roleId), Context.Locations, getLocationsCallback);
        }

        public void TryGeocode(string searchText, Action<IEnumerable<GeocoderResult>> geocoderResultsCallback)
        {
            Context.TryGeocode(searchText, callback => geocoderResultsCallback(callback.Value), null);
        }
    }
}

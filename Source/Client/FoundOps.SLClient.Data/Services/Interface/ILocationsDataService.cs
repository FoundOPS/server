using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.NET;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Context.Services.Interface
{
    public interface ILocationsDataService : IDataService
    {
        void GetLocations(Guid roleId, Action<ObservableCollection<Location>> getLocationsCallback);

        void TryGeocode(string searchText, Action<IEnumerable<GeocoderResult>> geocoderResultsCallback);
    }
}

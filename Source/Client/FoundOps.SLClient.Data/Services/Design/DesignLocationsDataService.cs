using System;
using System.Collections.Generic;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Context.Services.Interface;
using System.Collections.ObjectModel;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.MVVM.Services;
using FoundOps.Core.Models.CoreEntities.DesignData;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.Core.Context.Services.Design
{
    [ExportService(ServiceType.DesignTime, typeof(ILocationsDataService))]
    public class DesignLocationsDataService : DesignDataService, ILocationsDataService
    {
        readonly LocationsDesignData _locationsDesignData = new LocationsDesignData();

        public void GetLocations(Guid roleId, Action<ObservableCollection<Location>> getLocationsCallback)
        {
            var locations = new ObservableCollection<Location>
                                {
                                    _locationsDesignData.DesignLocation,
                                    _locationsDesignData.DesignLocationTwo,
                                    _locationsDesignData.DesignLocationThree
                                };

            getLocationsCallback(locations);
        }

        public void TryGeocode(string searchText, Action<IEnumerable<GeocoderResult>> geocoderResultsCallback)
        {
        }
    }
}

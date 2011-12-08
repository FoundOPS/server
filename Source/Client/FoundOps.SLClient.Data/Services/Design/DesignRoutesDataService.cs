using System;
using FoundOps.Core.Context.Services.Interface;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System.Collections.ObjectModel;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.MVVM.Services;
using MEFedMVVM.ViewModelLocator;

// Needs to be in the same namespace because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Context.Services.Design
// ReSharper restore CheckNamespace
{
    [ExportService(ServiceType.DesignTime, typeof(IRouteDataService))]
    public class DesignRoutesDataService : DesignDataService, IRouteDataService
    {
        readonly RoutesDesignData _routesDesignData = new RoutesDesignData();

        public void GetRoutesForServiceProviderOnDay(Guid roleId, DateTime dateOfRoutes, Action<ObservableCollection<Route>> getRoutesForServiceProviderOnDayCallback)
        {
            var routes = new ObservableCollection<Route>
                                    {
                                        _routesDesignData.DesignRoute,
                                        _routesDesignData.DesignRouteTwo,
                                        _routesDesignData.DesignRouteThree
                                    };

            getRoutesForServiceProviderOnDayCallback(routes);
        }

        public void GetUnroutedRouteTasks(Guid roleId, DateTime selectedDate, Action<ObservableCollection<RouteTask>> getUnroutedRouteTasksCallback)
        {
            var routeTasks = new ObservableCollection<RouteTask>
                           {
                               _routesDesignData.DesignRouteTask,
                               _routesDesignData.DesignRouteTaskTwo,
                               _routesDesignData.DesignRouteTaskThree
                           };

            getUnroutedRouteTasksCallback(routeTasks);
        }
    }
}

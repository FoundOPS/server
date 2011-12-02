using System;
using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Context.Services.Interface
{
    public interface IRouteDataService : IDataService
    {
        void GetRoutesForServiceProviderOnDay(Guid roleId, DateTime dateOfRoutes, Action<ObservableCollection<Route>> getRoutesForServiceProviderOnDayCallback);

        void GetUnroutedRouteTasks(Guid roleId, DateTime selectedDate, Action<ObservableCollection<RouteTask>> getUnroutedRouteTasksCallback);
    }
}

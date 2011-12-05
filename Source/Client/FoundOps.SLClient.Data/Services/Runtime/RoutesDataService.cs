using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using FoundOps.Core.Context.Services.Interface;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Windows.Data.DomainServices;
using ScalableCourier.Client.CommonResources.MVVM.Services;

namespace FoundOps.Core.Context.Services
{
    [ExportService(ServiceType.Runtime, typeof(IRouteDataService))]
    public class RoutesDataService : DomainContextDataService<CoreDomainContext>, IRouteDataService
    {
        [ImportingConstructor]
        public RoutesDataService(CoreDomainContext context)
            : base(context)
        {
        }

        public void GetRoutesForServiceProviderOnDay(Guid roleId, DateTime dateOfRoutes, Action<ObservableCollection<Route>> getRoutesForServiceProviderOnDayCallback)
        {
            var query = Context.GetRoutesForServiceProviderOnDayQuery(roleId, dateOfRoutes);

            Context.Load(query, (loadOperation) => getRoutesForServiceProviderOnDayCallback(new EntityList<Route>(Context.Routes, loadOperation.Entities)), null);
        }

        public void GetUnroutedRouteTasks(Guid roleId, DateTime selectedDate, Action<ObservableCollection<RouteTask>> getUnroutedRouteTasksCallback)
        {
            var query = Context.GetUnroutedRouteTasksQuery(roleId, selectedDate);

            Context.Load(query, (loadOperation) => getUnroutedRouteTasksCallback(new EntityList<RouteTask>(Context.RouteTasks, loadOperation.Entities)), null);
        }
    }
}

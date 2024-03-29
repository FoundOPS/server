﻿using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Displays the proper routes depending on the context.
    /// </summary>
    [ExportViewModel("RoutesInfiniteAccordionVM")]
    public class RoutesInfiniteAccordionVM : InfiniteAccordionVM<Route, Route>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesInfiniteAccordionVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RoutesInfiniteAccordionVM() : base(new[] { typeof(Employee), typeof(Vehicle) })
        {
            SetupContextDataLoading(roleId =>
                                        {
                                            var employeeContext = ContextManager.GetContext<Employee>();
                                            var employeeContextId = employeeContext != null ? employeeContext.Id : Guid.Empty;
                                            var vehicleContext = ContextManager.GetContext<Vehicle>();
                                            var vehicleContextId = vehicleContext != null ? vehicleContext.Id : Guid.Empty;

                                            return DomainContext.GetRouteLogForServiceProviderQuery(roleId, employeeContextId, vehicleContextId);
                                        }, null);
        }

        #region Logic

        protected override void OnAddEntity(Route newEntity)
        {
            throw new Exception("Not supposed to add routes in the RouteLog.");
        }

        protected override void OnDeleteEntity(Route newEntity)
        {
            throw new Exception("Not supposed to delete routes in the RouteLog.");
        }

        #endregion
    }
}

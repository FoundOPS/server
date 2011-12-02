using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using FoundOps.Core.Context.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;
using ScalableCourier.Client.CommonResources.MVVM.Services;

namespace FoundOps.SLClient.Data.Services.Runtime
{
    /// <summary>
    /// Deprecated. TODO Remove.
    /// Gets information for the navigator.
    /// </summary>
    [ExportService(ServiceType.Runtime, typeof(INavigatorDataService))]
    public class NavigatorDataService : DomainContextDataService<CoreDomainContext>, INavigatorDataService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorDataService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        [ImportingConstructor]
        public NavigatorDataService(CoreDomainContext context) : base(context)
        {
        }

        public void GetAvailableRoles(Action<ObservableCollection<Role>> getAvailableRolesCallback)
        {
            var query = Context.GetRolesQuery();
            Context.Load(query, loadOperation =>
                                    {
                                        if (!loadOperation.HasError)
                                        {
                                            getAvailableRolesCallback(new ObservableCollection<Role>(loadOperation.Entities));
                                        }
                                    }
                , null);
        }

        public void GetPublicBlocks(Action<ObservableCollection<Block>> getPublicBlocksCallback)
        {
            var query = Context.GetPublicBlocksQuery();
            Context.Load(query, loadOperation =>
            {
                if (!loadOperation.HasError)
                {
                    getPublicBlocksCallback(new ObservableCollection<Block>(loadOperation.Entities));
                }
            }
                , null);
        }
    }
}

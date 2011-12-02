using System;
using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Context.Services
{
    public interface INavigatorDataService : IDataService
    {
        void GetAvailableRoles(Action<ObservableCollection<Role>> getAvailableRolesCallback);
        void GetPublicBlocks(Action<ObservableCollection<Block>> getPublicBlocksCallback);
    }
}

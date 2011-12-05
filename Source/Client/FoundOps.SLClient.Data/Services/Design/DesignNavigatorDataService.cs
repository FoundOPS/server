using System;
using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.MVVM.Services;
using FoundOps.Core.Server.Blocks;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.Core.Context.Services.Design
{
    [ExportService(ServiceType.DesignTime, typeof(INavigatorDataService))]
    public class DesignNavigatorDataService : DesignDataService, INavigatorDataService
    {
        private readonly UserAccount _designUserAccount;
        private readonly BusinessAccount _designBusinessAccount;
        private readonly BusinessAccount _designBusinessAccountTwo;
        private readonly ObservableCollection<Role> _designRoles;

        private readonly BlocksData _blocksData = new BlocksData();

        public DesignNavigatorDataService()
        {
            _designUserAccount = new UserAccount { FirstName = "Jon", LastName = "Perl" };
            _designBusinessAccount = new BusinessAccount { Name = "Oren's Kosher Steakhouse" };

            _designBusinessAccountTwo = new BusinessAccount { Name = "Zach's Brews" };
            _designRoles = new ObservableCollection<Role>();
            var managerBlocks = _blocksData.ManagerBlocks;
            var userAdministratorBlocks = _blocksData.UserAccountBlocks;

            var managerRole = new Role { Name = "Manager", OwnerParty = _designBusinessAccount };
            managerRole.MemberParties.Add(_designUserAccount);

            foreach (var block in managerBlocks)
            {
                block.Link = "NotEmptyForDesignData";
                managerRole.Blocks.Add(block);
            }
            _designRoles.Add(managerRole);

            var userAdministratorRole = new Role { Name = "Account Settings", OwnerParty = _designUserAccount };
            foreach (var block in userAdministratorBlocks)
            {
                block.Link = "NotEmptyForDesignData";
                userAdministratorRole.Blocks.Add(block);
            }
            _designRoles.Add(userAdministratorRole);
        }

        public void GetAvailableRoles(Action<ObservableCollection<Role>> getAvailableRolesCallback)
        {
            getAvailableRolesCallback(_designRoles);
        }

        public void GetPublicBlocks(Action<ObservableCollection<Block>> getPublicBlocksCallback)
        {
            getPublicBlocksCallback(new ObservableCollection<Block>(_blocksData.PublicBlocks));
        }

        public void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback)
        {
            getCurrentUserAccountCallback(_designUserAccount);
        }

        public void GetUserAccount(Guid userAccountId, Action<UserAccount> getUserAccountCallback)
        {
            getUserAccountCallback(_designUserAccount);
        }
    }
}

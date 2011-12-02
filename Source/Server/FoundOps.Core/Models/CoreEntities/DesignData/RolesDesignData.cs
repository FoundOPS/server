using System;
using System.Collections.Generic;
using FoundOps.Core.Server.Blocks;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RolesDesignData
    {
        private readonly BusinessAccountsDesignData _businessAccountsDesignData;
        private readonly UserAccountsDesignData _userAccountsDesignData;
        private readonly BlocksData _blocksData;

        public Role FoundOpsAdministratorRole { get; private set; }
        public Role GotGreaseAdministratorRole { get; private set; }
        public Role ABCouriersAdministratorRole { get; private set; }
        public Role OrensKosherSteakhouseAdministratorRole { get; private set; }

        public List<Role> DesignBusinessAccountRoles { get; private set; }
        public List<Role> DesignUserAccountRoles { get; private set; }

        public RolesDesignData()
            : this(new BusinessAccountsDesignData(), new UserAccountsDesignData(), new BlocksData())
        {
        }

        public RolesDesignData(BusinessAccountsDesignData businessAccountsDesignData, UserAccountsDesignData userAccountsDesignData, BlocksData blocksData)
        {
            _businessAccountsDesignData = businessAccountsDesignData;
            _userAccountsDesignData = userAccountsDesignData;
            _blocksData = blocksData;

            DesignBusinessAccountRoles = new List<Role>();
            DesignUserAccountRoles = new List<Role>();

            InitializeDefaultRoles();

            //Setup Specific User Roles

            #region FoundOPS

            FoundOpsAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);

            #endregion

            #region GotGrease

            GotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.David);
            GotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Linda);
            GotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Terri);

            GotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);
            GotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Oren);

            #endregion

            #region Oren's Kosher Steakhouse

            OrensKosherSteakhouseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Andrew);
            OrensKosherSteakhouseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);
            OrensKosherSteakhouseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Oren);
            OrensKosherSteakhouseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Zach);

            #endregion

            #region AB Couriers

            ABCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.AlanMcClure);
            ABCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Andrew);
            ABCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);
            ABCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Oren);
            ABCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Zach);

            #endregion
        }

        private void InitializeDefaultRoles()
        {
            //Setup FoundOPS' Role
            FoundOpsAdministratorRole = new Role { Name = "Administrator", OwnerParty = BusinessAccountsDesignData.FoundOps };

            foreach (var adminConsoleBlock in _blocksData.AdministrativeConsoleBlocks)
            {
                FoundOpsAdministratorRole.Blocks.Add(adminConsoleBlock);
            }

            //Setup ServiceProvider's default admin role

            foreach (var serviceProvider in _businessAccountsDesignData.DesignServiceProviders)
            {
                var defaultServiceProviderRole = SetupServiceProviderAdministratorRole(serviceProvider);
                DesignBusinessAccountRoles.Add(defaultServiceProviderRole);

                if (serviceProvider == _businessAccountsDesignData.GotGrease)
                    GotGreaseAdministratorRole = defaultServiceProviderRole;

                if (serviceProvider == _businessAccountsDesignData.ABCouriers)
                    ABCouriersAdministratorRole = defaultServiceProviderRole;

                if (serviceProvider == _businessAccountsDesignData.OrenKosherSteakhouse)
                    OrensKosherSteakhouseAdministratorRole = defaultServiceProviderRole;
            }


            //Setup UserAccounts' default UserAccountRole
            foreach (var userAccount in _userAccountsDesignData.DesignUserAccounts)
                DesignUserAccountRoles.Add(SetupDefaultUserAccountRole(userAccount));
        }

        private Role SetupServiceProviderAdministratorRole(BusinessAccount ownerParty)
        {
            var role = new Role { Name = "Administrator", OwnerParty = ownerParty };

            foreach (var block in _blocksData.ManagerBlocks)
                role.Blocks.Add(block);

            foreach (var block in _blocksData.BusinessAdministratorBlocks)
                role.Blocks.Add(block);

            return role;
        }

        private Role SetupDefaultUserAccountRole(UserAccount userAccount)
        {
            var userRole = new Role
                               {
                                   Name = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName),
                                   OwnerParty = userAccount
                               };

            foreach (var block in _blocksData.UserAccountBlocks)
            {
                userRole.Blocks.Add(block);
            }

            return userRole;
        }
    }
}
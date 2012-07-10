using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RolesDesignData
    {
        private readonly BusinessAccountsDesignData _businessAccountsDesignData;
        private readonly UserAccountsDesignData _userAccountsDesignData;

        public Role FoundOpsAdministratorRole { get; private set; }
        public Role GotGreaseAdministratorRole { get; private set; }
        public Role ABCouriersAdministratorRole { get; private set; }
        public Role OrensKosherSteakhouseAdministratorRole { get; private set; }

        public RolesDesignData()
            : this(new BusinessAccountsDesignData(), new UserAccountsDesignData())
        {
        }

        public RolesDesignData(BusinessAccountsDesignData businessAccountsDesignData, UserAccountsDesignData userAccountsDesignData)
        {
            _businessAccountsDesignData = businessAccountsDesignData;
            _userAccountsDesignData = userAccountsDesignData;

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
            FoundOpsAdministratorRole = new Role { Name = "Administrator", OwnerParty = BusinessAccountsDesignData.FoundOps, RoleType = RoleType.Administrator };

            foreach (var adminConsoleBlock in BlocksData.AdministrativeConsoleBlocks)
            {
                FoundOpsAdministratorRole.Blocks.Add(adminConsoleBlock);
            }

            //Setup ServiceProvider's default admin role, and mobile role
            foreach (var serviceProvider in _businessAccountsDesignData.DesignServiceProviders)
            {
                //Setup the admin role
                var serviceProviderAdminRole = SetupServiceProviderRole(serviceProvider, RoleType.Administrator);

                if (serviceProvider == _businessAccountsDesignData.GotGrease)
                    GotGreaseAdministratorRole = serviceProviderAdminRole;

                if (serviceProvider == _businessAccountsDesignData.ABCouriers)
                    ABCouriersAdministratorRole = serviceProviderAdminRole;

                if (serviceProvider == _businessAccountsDesignData.GenericOilCollector)
                    OrensKosherSteakhouseAdministratorRole = serviceProviderAdminRole;

                //Setup the mobile role
                SetupServiceProviderRole(serviceProvider, RoleType.Mobile);
            }
        }

        public static Role SetupServiceProviderRole(BusinessAccount ownerParty, RoleType roleType)
        {
            var role = new Role { OwnerParty = ownerParty, RoleType = roleType };

            if (roleType == RoleType.Administrator)
            {
                role.Name = "Administrator";
                foreach (var block in BlocksData.ManagerBlocks.Union(BlocksData.HTMLBlocks))
                    role.Blocks.Add(block);
            }
            else if(roleType == RoleType.Mobile)
            {
                role.Name = "Mobile";
                foreach (var block in BlocksData.MobileBlocks)
                    role.Blocks.Add(block);
            }

            return role;
        }
    }
}
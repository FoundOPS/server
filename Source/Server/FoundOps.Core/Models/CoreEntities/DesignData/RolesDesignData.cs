using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RolesDesignData
    {
        private readonly BusinessAccountsDesignData _businessAccountsDesignData;
        private readonly UserAccountsDesignData _userAccountsDesignData;

        private Role _foundOpsAdministratorRole;

        private Role _gotGreaseAdministratorRole;
        private Role _gotGreaseMobileRole;

        private Role _abCouriersAdministratorRole;
        private Role _abCouriersMobileRole;

        private Role _genericOilCollectorAdminRole;
        private Role _genericOilCollectorMobileRole;

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

            _foundOpsAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);

            #endregion

            #region GotGrease

            _gotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.David);
            _gotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Linda);
            _gotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Terri);

            _gotGreaseAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);

            _gotGreaseMobileRole.MemberParties.Add(_userAccountsDesignData.Oren);

            #endregion

            #region Oren's Kosher Steakhouse

            _genericOilCollectorAdminRole.MemberParties.Add(_userAccountsDesignData.Andrew);
            _genericOilCollectorAdminRole.MemberParties.Add(_userAccountsDesignData.Jon);
            _genericOilCollectorAdminRole.MemberParties.Add(_userAccountsDesignData.Zach);

            _genericOilCollectorMobileRole.MemberParties.Add(_userAccountsDesignData.Oren);

            #endregion

            #region AB Couriers

            _abCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.AlanMcClure);
            _abCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Andrew);
            _abCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Jon);
            _abCouriersAdministratorRole.MemberParties.Add(_userAccountsDesignData.Zach);

            _abCouriersMobileRole.MemberParties.Add(_userAccountsDesignData.Oren);

            #endregion
        }

        private void InitializeDefaultRoles()
        {
            //Setup FoundOPS' Role
            _foundOpsAdministratorRole = new Role { Name = "Administrator", OwnerBusinessAccount = BusinessAccountsDesignData.FoundOps, RoleType = RoleType.Administrator };

            foreach (var adminConsoleBlock in BlocksData.AdministrativeConsoleBlocks)
            {
                _foundOpsAdministratorRole.Blocks.Add(adminConsoleBlock);
            }

            //Setup ServiceProvider's default admin role, and mobile role
            foreach (var serviceProvider in _businessAccountsDesignData.DesignServiceProviders)
            {
                //Setup the admin role
                var serviceProviderAdminRole = SetupServiceProviderRole(serviceProvider, RoleType.Administrator);

                //Setup the mobile role
                var serviceProviderMobileRole = SetupServiceProviderRole(serviceProvider, RoleType.Mobile);

                if (serviceProvider == _businessAccountsDesignData.GotGrease)
                {
                    _gotGreaseAdministratorRole = serviceProviderAdminRole;
                    _gotGreaseMobileRole = serviceProviderMobileRole;
                }
                else if (serviceProvider == _businessAccountsDesignData.ABCouriers)
                {
                    _abCouriersAdministratorRole = serviceProviderAdminRole;
                    _abCouriersMobileRole = serviceProviderMobileRole;
                }
                else if (serviceProvider == _businessAccountsDesignData.GenericOilCollector)
                {
                    _genericOilCollectorAdminRole = serviceProviderAdminRole;
                    _genericOilCollectorMobileRole = serviceProviderMobileRole;
                }
            }
        }

        public static Role SetupServiceProviderRole(BusinessAccount ownerParty, RoleType roleType)
        {
            var role = new Role { OwnerBusinessAccount = ownerParty, RoleType = roleType };

            if (roleType == RoleType.Administrator)
            {
                role.Name = "Administrator";
                foreach (var block in BlocksData.RegularBlocks.Union(BlocksData.RegularBlocks).Union(BlocksData.MobileBlocks))
                    role.Blocks.Add(block);
            }
            else if (roleType == RoleType.Mobile)
            {
                role.Name = "Mobile";
                foreach (var block in BlocksData.MobileBlocks)
                    role.Blocks.Add(block);
            }

            return role;
        }
    }
}
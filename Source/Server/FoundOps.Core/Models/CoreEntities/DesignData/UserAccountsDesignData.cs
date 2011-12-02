using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class UserAccountsDesignData
    {
        #region FoundOPS

        public UserAccount Andrew { get; private set; }
        public UserAccount Jon { get; private set; }
        public UserAccount Oren { get; private set; }
        public UserAccount Zach { get; private set; }

        #endregion

        #region GotGrease

        public UserAccount David { get; private set; }
        public UserAccount Linda { get; private set; }
        public UserAccount Terri { get; private set; }

        #endregion

        #region AB Couriers

        public UserAccount AlanMcClure { get; private set; }

        #endregion

        public IEnumerable<UserAccount> DesignUserAccounts { get; private set; }

        public UserAccountsDesignData()
        {
            InitializeUserAccounts();
        }

        private void InitializeUserAccounts()
        {
            #region FoundOPS

            Andrew = new UserAccount
            {
                Id = new Guid("14465d04-fe81-41ac-b169-489166f07cb9"),
                EmailAddress = "apohl@foundops.com",
                FirstName = "Andrew",
                LastName = "Pohl",
            };

            Jon = new UserAccount
            {
                Id = new Guid("14565d04-fe81-41ac-b169-489166f07cb9"),
                EmailAddress = "jperl@foundops.com",
                FirstName = "Jonathan",
                LastName = "Perl"
            };

            Oren = new UserAccount
            {
                Id = new Guid("14575d04-fe81-41ac-b169-489166f07cb9"),
                EmailAddress = "oshatken@foundops.com",
                FirstName = "Oren",
                LastName = "Shatken"
            };

            Zach = new UserAccount
            {
                Id = new Guid("14505d04-fe81-41ac-b169-489166f07cb9"),
                EmailAddress = "zbright@foundops.com",
                FirstName = "Zach",
                LastName = "Bright"
            };

            #endregion

            #region GotGrease

            David = new UserAccount
             {
                 Id = new Guid("8314b786-4aa4-4250-9760-22b6ac632d01"),
                 EmailAddress = "david@gotgrease.net",
                 FirstName = "David",
                 LastName = "Levenson"
             };

            Linda = new UserAccount
            {
                Id = new Guid("14595d04-fe81-41ac-b169-489166f07cb9"),
                EmailAddress = "linda@gotgrease.net",
                FirstName = "Linda",
                LastName = "Levenson"
            };

            Terri = new UserAccount
            {
                Id = new Guid("{E6655B6E-597B-4D1B-BA62-1BA1565E4CBD}"),
                EmailAddress = "terri@gotgrease.net",
                FirstName = "Terri",
                LastName = "Last"
            };

            #endregion

            #region AB Couriers

            AlanMcClure = new UserAccount
            {
                Id = new Guid("22BC442B-982B-4EE6-B72D-DCA25512B0CA"),
                EmailAddress = "alanmcclure93@gmail.com",
                FirstName = "Alan",
                LastName = "McClure"
            };

            #endregion

            DesignUserAccounts = new List<UserAccount> { Andrew, Jon, Oren, Zach, David, Linda, Terri, AlanMcClure };
        }
    }
}

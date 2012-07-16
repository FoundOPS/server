using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Entities;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Party : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Party()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        public virtual void OnCreate()
        {
            Id = Guid.NewGuid();
            var userAccount = this as UserAccount;
            if (userAccount != null)
            {
                userAccount.FirstName = "";
                userAccount.LastName = "";
                userAccount.MiddleInitial = "";
            }
            OnCreation();
        }

        #endregion

        public virtual string DisplayName
        {
            get
            {
                return "";
            }
            set { }
        }

        public IEnumerable<Role> AccessibleRoles
        {
            get 
            {
                return this is BusinessAccount
                           ? this.RoleMembership.Union(((BusinessAccount) this).OwnedRoles)
                           : this.RoleMembership;
            }
        }

        public IEnumerable<Block> AccessibleBlocks
        {
            get
            {
                return this.AccessibleRoles.SelectMany(r => r.Blocks).OrderBy(b => b.Name);
            }
        }
    }
}
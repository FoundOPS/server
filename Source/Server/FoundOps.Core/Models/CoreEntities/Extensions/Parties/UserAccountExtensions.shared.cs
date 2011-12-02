using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    //NOTE: Not shared because it is not editable on the client side yet
    public partial class UserAccount : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public UserAccount()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
            OnCreation();
        }

        #endregion
    }
}
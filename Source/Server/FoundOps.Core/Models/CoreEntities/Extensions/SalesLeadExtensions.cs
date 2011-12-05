using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class SalesLead : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

        public SalesLead()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion
    }
}
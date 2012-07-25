using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Entities;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public enum RoleType
    {
        Administrator = 0,
        Custom = 1,
        Mobile = 2,
        Regular = 3
    }

    public partial class Role : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Role()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion

        public RoleType RoleType
        {
            get
            {
                return (RoleType)Convert.ToInt32(this.RoleTypeInt);
            }
            set
            {
                this.RoleTypeInt = Convert.ToInt16(value);
            }
        }
    }
}
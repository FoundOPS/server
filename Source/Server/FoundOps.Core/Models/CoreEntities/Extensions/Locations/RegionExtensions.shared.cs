﻿using System;
using FoundOps.Common.Composite.Entities;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Region : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Region()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            //Need to have a name that is not empty, or else Ria Services will convert it to null and throw a validation error
            Name = "New Region";
            Notes = "";
            CreatedDate = DateTime.UtcNow;
            OnCreation();
        }

        #endregion
    }
}
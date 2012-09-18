﻿using System;
using FoundOps.Common.Composite.Entities;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Location : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Location()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif
        partial void OnCreation(); //For Extensions on Silverlight Side

        public void OnCreate()
        {
            this.Id = Guid.NewGuid();
            //To prevent random HasChanges
            this.Name = "";
            this.AddressLineOne = "";
            this.AddressLineTwo = "";
            this.City = "";
            this.State = "";
            OnCreation();
        }

        public string AddressCity { get
        {
            if (AddressLineOne == "" && AddressLineTwo == "" && City == "")
                return "";
            if (AddressLineTwo == "" && City == "")
                return AddressLineOne;
            if (AddressLineOne == "" && AddressLineTwo == "")
                return City;
            if (AddressLineTwo == "")
                return AddressLineOne + ", " + City;
            return this.AddressLineOne + " " + this.AddressLineTwo + ", " + this.City;
        }}

        #region Implementation of ICompositeRaiseEntityPropertyChanged

#if SILVERLIGHT
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
#else
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
#endif
        #endregion
    }
}
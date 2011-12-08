using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
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
            this.Name = "New Location";
            //To prevent random HasChanges
            this.AddressLineOne = "";
            this.AddressLineTwo = "";
            this.City = "";
            this.State = "";
            OnCreation();
        }

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
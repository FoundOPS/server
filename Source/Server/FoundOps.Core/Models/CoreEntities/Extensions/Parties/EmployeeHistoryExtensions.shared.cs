using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class EmployeeHistoryEntry : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public EmployeeHistoryEntry()
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
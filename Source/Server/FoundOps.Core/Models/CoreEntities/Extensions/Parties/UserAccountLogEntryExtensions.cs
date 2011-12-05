using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public enum UserAccountLogType
    {
        Login,
        Logout,
        PasswordChange
    }

    public partial class UserAccountLogEntry : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public UserAccountLogEntry()
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

        partial void OnTypeIdChanged()
        {
#if SILVERLIGHT
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("UserAccountLogType"));      
#else
            this.OnPropertyChanged("UserAccountLogType");
#endif
        }

        public UserAccountLogType UserAccountLogType
        {
            get { return (UserAccountLogType)this.TypeId; }
            set { TypeId = (int)value; }
        }
    }
}
using FoundOps.Common.Composite.Entities;
using System;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public enum OptionsType
    {
        Combobox = 0,
        Checkbox = 1,
        Checklist = 2
    }

    public partial class OptionsField : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public OptionsField()
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

        public OptionsType OptionsType
        {
            get
            {
                return (OptionsType)Convert.ToInt32(this.TypeInt);
            }
            set
            {
                this.TypeInt = Convert.ToInt16(value);
            }
        }

        partial void OnTypeIntChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("OptionsType");
        }
    }
}
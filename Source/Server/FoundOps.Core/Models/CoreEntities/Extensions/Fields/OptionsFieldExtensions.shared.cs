using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
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

               //The way to clone an entity on the server is different then the way to clone on the client
        //That is why this is split into !SILVERLIGHT and SILVERLIGHT
#if !SILVERLIGHT
        public override Field MakeChild()
        {
            var child = (OptionsField) base.MakeChild();

            foreach (var option in this.Options)
            {
                //Clone with the reflection based extension method
                var clonedOption = option.Clone();

                //Update the Id
                clonedOption.Id = Guid.NewGuid();

                //Update the OptionsField the clonedOption belongs to
                clonedOption.OptionsField = child;
            }

            return child;
        }
#endif
    }
}
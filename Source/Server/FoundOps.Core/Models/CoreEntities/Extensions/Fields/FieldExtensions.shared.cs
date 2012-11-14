using FoundOps.Common.Composite.Entities;
using System;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// A generic Field to be extended. Not intended to be created.
    /// </summary>
    public partial class Field : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Field()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }

#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        public void OnCreate()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
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

        //The way to clone an entity on the server is different then the way to clone on the client
        //That is why this is split into !SILVERLIGHT and SILVERLIGHT
#if !SILVERLIGHT
        public virtual Field MakeChild()
        {
            //Clone with the reflection based extension method
            var fieldChild = this.Clone();

            //Update the Id
            fieldChild.Id = Guid.NewGuid();
            fieldChild.CreatedDate = DateTime.UtcNow;

            //Clear ServiceTemplate to prevent overriding ParentField's ServiceTemplate
            fieldChild.OwnerServiceTemplate = null;

            fieldChild.ParentField = this;

            return fieldChild;
        }
#endif
    }
}
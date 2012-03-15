using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using System;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Field
    {
        #region Public Properties

        /// <summary>
        /// Gets the this field. For bindings that require arbitrary NotifyPropertyChanged("ThisField") 
        /// </summary>
        public Field ThisField
        {
            get { return this; }
            set
            {
                //Do nothing. Set is required to allow for 2 way bindings
            }
        }

        #endregion

        #region Logic

        #region Overridden Methods

        /// <summary>
        /// Makes a child of the Field.
        /// </summary>
        /// <returns></returns>
        public virtual Field MakeChild()
        {
            return MakeChildSilverlight();
        }

        /// <summary>
        /// A method that works in silverlight for making a child of the Field.
        /// </summary>
        protected virtual Field MakeChildSilverlight()
        {
            //Clone using RIA Services Contrib's Entity Graph
            var fieldChild = this.Clone(new EntityGraphShape());

            //Update the Id
            fieldChild.Id = Guid.NewGuid();

            //Clear associations before adding it to the EntitySet
            fieldChild.ParentField = null;
            fieldChild.ParentFieldId = null;
            
            //Clear ServiceTemplate to prevent overriding ParentField's ServiceTemplate
            fieldChild.OwnerServiceTemplate = null;
            fieldChild.ServiceTemplateId = null;
         
            //Update the parent of the field
            fieldChild.ParentField = this;

            return fieldChild;
        }

        #endregion

        #endregion
    }
}

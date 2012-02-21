using System;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

//This is a partial class and must be in the same namespace as the generated CoreDomainService
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Field
    {
        public virtual Field MakeChild()
        {
            return MakeChildSilverlight();
        }

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
    }
}

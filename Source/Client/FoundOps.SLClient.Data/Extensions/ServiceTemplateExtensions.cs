using System;
using System.Linq;
using RiaServicesContrib.DomainServices.Client;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceTemplate
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);
            if (isInitialLoad)
            {
            }
        }

        /// <summary>
        /// Makes a child of this ServiceTemplate.
        /// </summary>
        /// <param name="serviceTemplateLevel">The service template level of the child.</param>
        /// <returns></returns>
        private ServiceTemplate MakeChildSilverlight(ServiceTemplateLevel serviceTemplateLevel)
        {
            var serviceTemplateChild = MakeFamilyMember();

            //Setup the ServiceTemplateLevel
            serviceTemplateChild.ServiceTemplateLevel = serviceTemplateLevel;

            //Set the ParentServiceTemplate to this
            serviceTemplateChild.ParentServiceTemplate = this;

            //Make children of each of this fields
            foreach (var fieldToCopy in this.Fields.ToArray())
            {
                var childField = fieldToCopy.MakeChild();

                //set the field's service template
                childField.OwnerServiceTemplate = serviceTemplateChild;
            }

            return serviceTemplateChild;
        }

        /// <summary>
        /// Similar to MakeChild. It copies the ServiceTemplate/Fields. It keeps the same ServiceTemplateLevel.
        /// It sets the parents to this ServiceTemplate's Parent and this Field's Parents
        /// </summary>
        public ServiceTemplate MakeSibling()
        {
            var sibling = this.MakeFamilyMember();

            //fix the parent reference
            sibling.ParentServiceTemplate = this.ParentServiceTemplate;

            //copy this fields
            foreach (var fieldToCopy in this.Fields.ToArray())
            {
                var childField = fieldToCopy.MakeChild();

                //set the field's parent to this field's parents
                childField.ParentField = fieldToCopy.ParentField;

                //set the field's service template
                childField.OwnerServiceTemplate = sibling;
            }

            return sibling;
        }

        private ServiceTemplate MakeFamilyMember()
        {
            var entityGraph = new EntityGraph(this, new RiaServicesContrib.EntityGraphShape());

            var serviceTemplateFamilyMember = (ServiceTemplate)entityGraph.Clone();

            if (serviceTemplateFamilyMember.ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined)
            {

            }

            if (serviceTemplateFamilyMember.ServiceTemplateLevel != ServiceTemplateLevel.ServiceProviderDefined)
            {
                serviceTemplateFamilyMember.OwnerServiceProvider = null;
                serviceTemplateFamilyMember.OwnerServiceProviderId = null;
            }

            if (serviceTemplateFamilyMember.ServiceTemplateLevel != ServiceTemplateLevel.ClientDefined)
            {
                serviceTemplateFamilyMember.OwnerClient = null;
                serviceTemplateFamilyMember.OwnerClientId = null;
            }

            if (serviceTemplateFamilyMember.ServiceTemplateLevel != ServiceTemplateLevel.RecurringServiceDefined)
                serviceTemplateFamilyMember.OwnerRecurringService = null;

            if (serviceTemplateFamilyMember.ServiceTemplateLevel == ServiceTemplateLevel.ServiceDefined)
                serviceTemplateFamilyMember.OwnerService = null;

            serviceTemplateFamilyMember.Id = Guid.NewGuid();

            return serviceTemplateFamilyMember;
        }
    }
}


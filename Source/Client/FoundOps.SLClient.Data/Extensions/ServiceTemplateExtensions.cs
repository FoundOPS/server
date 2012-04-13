using FoundOps.Common.Silverlight.UI.Interfaces;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceTemplate : ILoadDetails
    {
        #region Public Properties

        #region Implementation of ILoadDetails

        private bool _detailsLoaded;
        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                //Cannot clear details loaded. This is prevent issues when saving.
                if (_detailsLoaded)
                    return;

                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        #endregion

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
            var serviceTemplateFamilyMember = this.Clone(new EntityGraphShape());

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

    /// <summary>
    /// </summary>
    public static class ServiceTemplateHelpers
    {
        /// <summary>
        /// Compares two service templates to see if one is an ancestor of the other.
        /// </summary>
        /// <param name="a">One service template to compare.</param>
        /// <param name="b">The other service template to compare.</param>
        public static bool ServiceTemplateIsAncestorOrDescendent(ServiceTemplate a, ServiceTemplate b)
        {
            //Climb the parents of x and see if an ancestor is y
            var currentServiceTemplate = a;
            while (currentServiceTemplate != null)
            {
                if (currentServiceTemplate == b)
                    return true;

                currentServiceTemplate = currentServiceTemplate.ParentServiceTemplate;
            }

            //Climb the parents of y and see if an ancestor is x
            currentServiceTemplate = b;
            while (currentServiceTemplate != null)
            {
                if (currentServiceTemplate == a)
                    return true;

                currentServiceTemplate = currentServiceTemplate.ParentServiceTemplate;
            }

            return false;
        }
    }
}


using System;
using FoundOps.Common.Silverlight.Interfaces;
using RiaServicesContrib;
using System.ServiceModel.DomainServices.Client;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Service : IRaiseValidationErrors, IReject
    {
        #region Public Properties

        /// <summary>
        /// The parent ServiceHolder if this is a generated service. Otherwise it will be null.
        /// </summary>
        public ServiceHolder ParentServiceHolder { get; set; }

        #endregion

        #region Logic

        #region Public Methods

        /// <summary>
        /// Contains the Service, ServiceTemplate, and Fields
        /// </summary>
        public EntityGraph<Entity> EntityGraph()
        {
            var graphShape = new EntityGraphShape().Edge<Service, ServiceTemplate>(service => service.ServiceTemplate)
                .Edge<ServiceTemplate, Field>(st => st.Fields).Edge<OptionsField, Option>(of => of.Options);

            return new EntityGraph<Entity>(this, graphShape);
        }

        /// <summary>
        /// Raise validation errors for this entity.
        /// </summary>
        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }

        /// <summary>
        /// Rejects the changes of this individual entity.
        /// </summary>
        public void Reject()
        {
            this.RejectChanges();
        }

        #endregion

        #endregion
    }
}

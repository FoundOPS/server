using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib;
using FoundOps.Common.Silverlight.Interfaces;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Service : IRaiseValidationErrors, IReject
    {
        private bool _trackingChanges;


        private bool _serviceHasChanges;
        /// <summary>
        /// Allow for tracking changes
        /// Ex. Used in ServicesVM so that services generated from recurring services which do not have changes are not added to the database
        /// </summary>
        /// <value>
        ///   <c>true</c> if [service has changes]; otherwise, <c>false</c>.
        /// </value>
        public bool ServiceHasChanges
        {
            get { return _serviceHasChanges; }
            set
            {
                _serviceHasChanges = value;
                this.RaisePropertyChanged("ServiceHasChanges");
            }
        }

       
        /// <summary>
        /// Allow for tracking if this was New this session
        /// Ex. Used in ServicesVM so that New entities with ServiceHasChanges == false can be removed
        /// </summary>
        public bool ServiceIsNew { get; set; }

        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }

        public void Reject()
        {
            this.RejectChanges();
        }

        public EntityGraph<Entity> EntityGraph
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Service, ServiceTemplate>(service => service.ServiceTemplate).Edge
                        <ServiceTemplate, Field>(st => st.Fields).Edge<OptionsField, Option>(of => of.Options);

                return new EntityGraph<Entity>(this, graphShape);
            }
        }

        /// <summary>
        /// Track this Entity's Changes
        /// </summary>
        public void TrackChanges()
        {
            if (_trackingChanges)
                return;

            _trackingChanges = true;

            //Track changes on this service
            this.EntityGraph.PropertyChanged += (sender, e) =>
            {
                //Check if the property that is changing is a DataMember, If not do not consider it as a change
                if (!sender.GetType().GetProperties().Any(pi => pi.Name == e.PropertyName && pi.IsDefined(typeof(System.Runtime.Serialization.DataMemberAttribute), true)))
                    return;

                ServiceHasChanges = true;
            };
        }

        ///<summary>
        /// This Service's service type. The property is used for Service's who's ServiceTemplate isn't generated yet.
        ///</summary>
        public string ServiceType
        {
            get
            {
                if (this.ServiceTemplate != null)
                    return this.ServiceTemplate.Name;
                
                if (this.Generated && this.RecurringServiceParent != null && this.RecurringServiceParent.ServiceTemplate!=null)
                    return this.RecurringServiceParent.ServiceTemplate.Name;

                return "";
            }
        }
    }
}

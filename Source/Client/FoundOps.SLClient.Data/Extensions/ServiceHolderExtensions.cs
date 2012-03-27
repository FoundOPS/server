using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Services;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceHolder : ILoadDetails, IRaiseValidationErrors, IReject
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
                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        /// <summary>
        /// The loaded or generated service.
        /// </summary>
        public Service Service { get; set; }

        #endregion

        #region Logic

        #region Public

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

        /// <summary>
        /// Load or generate the Service.
        /// </summary>
        public void LoadDetails()
        {
            if (ServiceId != null)
            {
                Manager.Data.LoadSingle(Manager.CoreDomainContext.GetServiceDetails());
            }

        }

        #endregion

        #endregion


        #region Service Changes logic

        //private bool _trackingChanges;

        //private bool _serviceHasChanges;
        ///// <summary>
        ///// Tracks whether or not the generated or related service has changes.
        ///// </summary>
        ///// <value>
        /////   <c>true</c> if [service has changes]; otherwise, <c>false</c>.
        ///// </value>
        //public bool ServiceHasChanges
        //{
        //    get { return _serviceHasChanges; }
        //    set
        //    {
        //        _serviceHasChanges = value;
        //        this.RaisePropertyChanged("ServiceHasChanges");
        //    }
        //}

        //public Service GeneratedService { get; set; }

        ///// <summary>
        ///// Allow for tracking if this was New this session
        ///// Ex. Used in ServicesVM so that New entities with ServiceHasChanges == false can be removed
        ///// </summary>
        //public bool ServiceIsNew { get; set; }

        //public EntityGraph<Entity> EntityGraph
        //{
        //    get
        //    {
        //        var graphShape =
        //            new EntityGraphShape().Edge<Service, ServiceTemplate>(service => service.ServiceTemplate).Edge
        //                <ServiceTemplate, Field>(st => st.Fields).Edge<OptionsField, Option>(of => of.Options);

        //        return new EntityGraph<Entity>(this, graphShape);
        //    }
        //}

        ///// <summary>
        ///// Track this Entity's Changes
        ///// </summary>
        //public void TrackChanges()
        //{
        //    if (_trackingChanges)
        //        return;

        //    _trackingChanges = true;

        //    //Track changes on this service
        //    this.EntityGraph.PropertyChanged += (sender, e) =>
        //    {
        //        //Check if the property that is changing is a DataMember, If not do not consider it as a change
        //        if (!sender.GetType().GetProperties().Any(pi => pi.Name == e.PropertyName && pi.IsDefined(typeof(System.Runtime.Serialization.DataMemberAttribute), true)))
        //            return;

        //        ServiceHasChanges = true;
        //    };
        //}

        /////<summary>
        ///// This Service's service type. The property is used for Service's whos ServiceTemplate isn't generated yet.
        /////</summary>
        //public string ServiceType
        //{
        //    get
        //    {
        //        if (this.ServiceTemplate != null)
        //            return this.ServiceTemplate.Name;
                
        //        if (this.Generated && this.RecurringServiceParent != null && this.RecurringServiceParent.ServiceTemplate!=null)
        //            return this.RecurringServiceParent.ServiceTemplate.Name;

        //        return "";
        //    }
        //}

        #endregion
    }
}

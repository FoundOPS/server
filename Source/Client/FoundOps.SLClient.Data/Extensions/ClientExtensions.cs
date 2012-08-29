using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;
using System;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Client : IReject, ILoadDetails
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

        /// <summary>
        /// Gets the entity graph of Client to remove.
        /// </summary>
        public EntityGraph<Entity> EntityGraphToRemove
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Client, ContactInfo>(client => client.ContactInfoSet)
                    .Edge<Client, ServiceTemplate>(client => client.ServiceTemplates).Edge<ServiceTemplate, Field>(st => st.Fields)
                    .Edge<OptionsField, Option>(of => of.Options);

                return new EntityGraph<Entity>(this, graphShape);
            }
        }

        #endregion

        #region Locals

        //Keep track of when this has been initialized so it only happens once
        private bool _initialized;

        private IDisposable _displayNameSubscription;

        #endregion

        #region Constructor / Initialization

        partial void OnCreation()
        {
            InitializeHelper();
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                InitializeHelper();

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            if (_initialized)
                return;

            //Clear the last subscriptions and handlers
            ClearSubscriptionsHandlers();

            //Setup DefaultBillingLocation whenever a Location is added to this client
            this.Locations.EntityAdded += LocationsEntityAdded;

            //Setup DefaultBillingLocation whenever a Location is removed from this client
            this.Locations.EntityRemoved += LocationsEntityRemoved;


            //Keep the location name synchronized if there is only one location
            //and it had the same name as the Client
            //var lastName = this.Name;
            //Observable2.FromPropertyChangedPattern(this, x => x.Name).ObserveOnDispatcher()
            //    .Subscribe(name =>
            //    {
            //        var defaultLocation = this.Locations.Count == 1 ? this.Locations.First() : null;
            //        if (defaultLocation == null) return;

            //        if (lastName == defaultLocation.Name)
            //            defaultLocation.Name = name;

            //        lastName = name;
            //    });

            _initialized = true;
        }


        private void ClearSubscriptionsHandlers()
        {
            if (_displayNameSubscription != null)
            {
                _displayNameSubscription.Dispose();
                _displayNameSubscription = null;
            }

            this.Locations.EntityAdded -= LocationsEntityAdded;
            this.Locations.EntityRemoved -= LocationsEntityRemoved;
        }

        void LocationsEntityAdded(object sender, EntityCollectionChangedEventArgs<Location> e)
        {
            var exists = this.Locations.FirstOrDefault(l => l.IsDefaultBillingLocation == true);
            //Only set the DefaultBillingLocation if one does not already exist
            if (exists != null) 
                exists.IsDefaultBillingLocation = true; 
        }

        void LocationsEntityRemoved(object sender, EntityCollectionChangedEventArgs<Location> e)
        {
            //Remove the BillingLocation from this client if it is a billing location
            if (e.Entity.IsDefaultBillingLocation == true)
                e.Entity.IsDefaultBillingLocation = true;

            var exists = this.Locations.FirstOrDefault(l => l.IsDefaultBillingLocation == true);
 
            //If there is no DefaultBillingLocation. Set the DefaultBillingLocation to the first location this client 
            if (exists != null)
                exists.IsDefaultBillingLocation = true; 
        }


        #endregion

        #region Logic

        #region Public Methods

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

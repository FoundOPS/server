using System;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.Interfaces;
using RiaServicesContrib;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using FoundOps.Common.Silverlight.Interfaces;

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
                    new EntityGraphShape().Edge<Client, Party>(client => client.OwnedParty).Edge<Party, ContactInfo>(ownedParty => ownedParty.ContactInfoSet)
                    .Edge<Client, ServiceTemplate>(client => client.ServiceTemplates).Edge<ServiceTemplate, Field>(st => st.Fields)
                    .Edge<OptionsField, Option>(of => of.Options);

                return new EntityGraph<Entity>(this, graphShape);
            }
        }

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
            if (this.OwnedParty != null)
                OwnedPartyOperations();

            //Follow OwnedParty changes
            Observable2.FromPropertyChangedPattern(this, x => x.OwnedParty)
                .ObserveOnDispatcher().Subscribe(_ => OwnedPartyOperations());
        }

        private void OwnedPartyOperations()
        {
            //TODO make all disposable

            if (this.OwnedParty == null) return;

            //Update this DisplayName whenever OwnedParty.DisplayName changes
            Observable2.FromPropertyChangedPattern(this.OwnedParty, x => x.DisplayName).DistinctUntilChanged()
            .ObserveOnDispatcher().Subscribe(_ => this.CompositeRaiseEntityPropertyChanged("DisplayName"));

            //Setup DefaultBillingLocation whenever a Location is added to this client's OwnedParty
            this.OwnedParty.Locations.EntityAdded += (s, e) =>
            {
                //Only set the DefaultBillingLocation if one does not already exist
                if (!this.DefaultBillingLocationId.HasValue)
                    this.DefaultBillingLocation = this.OwnedParty.Locations.FirstOrDefault();
            };

            //Setup DefaultBillingLocation whenever a Location is removed from this client's OwnedParty
            this.OwnedParty.Locations.EntityRemoved += (s, e) =>
            {
                //Remove the BillingLocation from this client if it is a billing location
                if (e.Entity.ClientsWhereBillingLocation.Contains(this))
                    e.Entity.ClientsWhereBillingLocation.Remove(this);

                //If there is no DefaultBillingLocation. Set the DefaultBillingLocation to the first location this client 
                if (!this.DefaultBillingLocationId.HasValue)
                    this.DefaultBillingLocation = this.OwnedParty.Locations.FirstOrDefault();
            };

            ////TODO: REDO
            //var rejectionHandled = true;
            //DomainContext.ChangesRejected += (s, e) => rejectionHandled = false;

            ////Whenever the client name changes update the default location name
            ////There is a default location as long as there is only one location
            //SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null)
            //.SelectLatest(selectedClient => Observable2.FromPropertyChangedPattern(selectedClient, x => x.DisplayName).DistinctUntilChanged())
            //.Subscribe(displayName =>
            //{
            //    //Ignore changes after the DomainContext rejects changes
            //    if (!rejectionHandled)
            //    {
            //        rejectionHandled = true;
            //        return;
            //    }

            //    //If the Client only has one location 
            //    //and if that location had the same name as the client
            //    //update it's name whenever the client's name changes
            //    var defaultLocation = SelectedEntity.OwnedParty.Locations.Count == 1 ? SelectedEntity.OwnedParty.Locations.First() : null;
            //    if (defaultLocation == null) return;
            //    defaultLocation.Name = displayName;
            //});
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

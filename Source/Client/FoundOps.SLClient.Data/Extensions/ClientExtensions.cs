using System;
using System.Reactive.Linq;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Interfaces;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Client : IReject
    {
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
            //Whenever OwnedParty's DisplayName changes, update this DisplayName
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "OwnedParty") return;
                if (this.OwnedParty == null) return;
                OwnedParty.PropertyChanged += (s, args) =>
                {
                    if(args.PropertyName!= "DisplayName") return;
                    this.CompositeRaiseEntityPropertyChanged("DisplayName");
                };
            };

            if(this.OwnedParty!=null)
                OwnedParty.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName != "DisplayName") return;
                    this.CompositeRaiseEntityPropertyChanged("DisplayName");
                };
        }

        public void Reject()
        {
            this.RejectChanges();
        }

        /// <summary>
        /// Gets the entity graph of Client to remove.
        /// </summary>
        public EntityGraph EntityGraphToRemove
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Client, Party>(client => client.OwnedParty).Edge<Party, ContactInfo>(ownedParty => ownedParty.ContactInfoSet)
                    .Edge<Client, ServiceTemplate>(client => client.ServiceTemplates).Edge<ServiceTemplate, Field>(st => st.Fields)
                    .Edge<OptionsField, Option>(of => of.Options);

                return new EntityGraph(this, graphShape);
            }
        }
    }
}

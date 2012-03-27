using System;
using ReactiveUI;
using System.Linq;
using ReactiveUI.Xaml;
using System.ComponentModel;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;


namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A VM which manages Contact's ClientTitles
    /// </summary>
    [ExportViewModel("ClientTitlesVM")]
    public class ClientTitlesVM : InfiniteAccordionVM<ClientTitle>
    {
        //Public

        /// <summary>
        /// Gets the distinct titles.
        /// </summary>
        public IEnumerable<string> DistinctTitles
        {
            get { return DomainContext.ClientTitles.Select(t => t.Title).Distinct().OrderBy(t => t); }
        }

        //Local
        private ClientTitle _clientTitleInCreation;
        /// <summary>
        /// Only public for this.WhenAny. TODO, update to use AddToDeleteFrom
        /// </summary>
        public ClientTitle ClientTitleInCreation
        {
            get { return _clientTitleInCreation; }
            set
            {
                PropertyChangedEventHandler raiseValidationErrors = (sender, e) =>
                {
                    if (e.PropertyName == "Client" || e.PropertyName == "Title")
                    {
                        //Whenever the ClientTitleInCreation's ClientId or Title changed update its validation errors
                        ClientTitleInCreation.RaiseValidationErrors();
                    }
                };

                if (ClientTitleInCreation != null)
                    ClientTitleInCreation.PropertyChanged -= raiseValidationErrors;

                _clientTitleInCreation = value;

                this.RaisePropertyChanged("ClientTitleInCreation");

                if (ClientTitleInCreation != null)
                    ClientTitleInCreation.PropertyChanged += raiseValidationErrors;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTitlesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ClientTitlesVM() : base(new [] { typeof(Contact) })
        {
            //TODO Optimization
            //SetupMainQuery(DataManager.Query.ClientTitles, entities => this.RaisePropertyChanged("DistinctTitles"), "Title");

            #region Register Commands

            //Override the behavior of the AddCommand

            //CanExecute AddCommand relies on ClientTitleInCreation not having validation errors 
            var canAdd = this.WhenAny(x => x.ClientTitleInCreation.HasValidationErrors, hasValidationErrors => !hasValidationErrors.Value);

            AddCommand = new ReactiveCommand(canAdd);
            AddCommand.Subscribe(cmdParam =>
            {
                if (ClientTitleInCreation == null) return;

                //Add the ClientTitleInCreation to the DCV
                ((IList<ClientTitle>)this.CollectionView.SourceCollection).Add(ClientTitleInCreation);
                ClientTitleInCreation = null;
            });

            #endregion
        }

        #region Logic

        //Public

        /// <summary>
        /// Deletes the client title in creation.
        /// </summary>
        public void DeleteClientTitleInCreation()
        {
            this.DomainContext.ClientTitles.Remove(ClientTitleInCreation);
            ClientTitleInCreation = null;
        }

        /// <summary>
        /// Starts the creation of client title.
        /// </summary>
        /// <returns></returns>
        public ClientTitle StartCreationOfClientTitle()
        {
            ClientTitleInCreation = new ClientTitle { Contact = ContextManager.GetContext<Contact>() };
            ClientTitleInCreation.RaiseValidationErrors();
            return ClientTitleInCreation;
        }

        //Overridden

        //protected override bool EntityIsPartOfView(ClientTitle entity, bool isNew)
        //{
        //    var contactContext = ContextManager.GetContext<Contact>();

        //    if (contactContext == null) return true;

        //    return entity.ContactId == contactContext.Id;
        //}


        #endregion
    }
}

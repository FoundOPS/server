using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying BusinessAccounts
    /// </summary>
    [ExportViewModel("BusinessAccountsVM")]
    public class BusinessAccountsVM : CoreEntityCollectionInfiniteAccordionVM<Party>, IAddToDeleteFromProvider //Base class is Party because DomainCollectionView does not work well with inheritance
    {
        #region Public

        #region Implementation of IAddToDeleteFromProvider

        private readonly Action<string> _addNewItemFromString;
        public Action<string> AddNewItemFromString { get { return _addNewItemFromString; } }

        private readonly Action<object> _addExistingItem;
        public Action<object> AddExistingItem { get { return _addExistingItem; } }

        public IEnumerable DestinationItemsSource { get { return SelectedEntity.FirstOwnedRole.MemberParties; } }

        #endregion

        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(BusinessAccount); }
        }

        private ContactInfoVM _selectedEntityContactInfoVM;
        /// <summary>
        /// Gets or sets the selected entity contact info VM.
        /// </summary>
        /// <value>
        /// The selected entity contact info VM.
        /// </value>
        public ContactInfoVM SelectedEntityContactInfoVM
        {
            get { return _selectedEntityContactInfoVM; }
            set
            {
                _selectedEntityContactInfoVM = value;
                this.RaisePropertyChanged("SelectedEntityContactInfoVM");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public BusinessAccountsVM(DataManager dataManager)
            : base(dataManager, true)
        {
            SetupMainQuery(DataManager.Query.BusinessAccounts);

            #region Implementation of IAddToDeleteFromSource

            //Whenever the SelectedEntity changes, notify the DestinationItemsSource changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(
                _ => this.RaisePropertyChanged("DestinationItemsSource"));

            _addNewItemFromString = name =>
            {
                var newUserAccount = (Party)VM.UserAccounts.CreateNewItemFromString(name);
                this.SelectedEntity.FirstOwnedRole.MemberParties.Add(newUserAccount);
            };

            _addExistingItem = existingItem =>
            {
                this.SelectedEntity.FirstOwnedRole.MemberParties.Add((Party) existingItem);
            };

            #endregion

        }

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            var newBusinessAccount = new BusinessAccount();
            ((EntityList<Party>)this.DomainCollectionView.SourceCollection).Add(newBusinessAccount);
            return newBusinessAccount;
        }

        /// <summary>
        /// Called when [selected entity changed]. 
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnSelectedEntityChanged(Party oldValue, Party newValue)
        {
            base.OnSelectedEntityChanged(oldValue, newValue);

            if (newValue != null)
                //Whenever the SelectedEntity changes setup the ContactInfoVM for that entity
                SelectedEntityContactInfoVM = new ContactInfoVM(DataManager, ContactInfoType.OwnedParties, newValue.ContactInfoSet);
        }

        #endregion
    }
}

using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Common.Silverlight.Tools;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccounts
    /// </summary>
    [ExportViewModel("UserAccountsVM")]
    public class UserAccountsVM : CoreEntityCollectionInfiniteAccordionVM<Party> //Base class is Party because DomainCollectionView does not work well with inheritance
    {
        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(UserAccount); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public UserAccountsVM(DataManager dataManager)
            : base(dataManager, true)
        {
            this.SetupMainQuery(DataManager.Query.UserAccounts);
        }

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            var newUserAccount = new UserAccount { TemporaryPassword = PasswordTools.GeneratePassword() };
            ((EntityList<Party>)this.DomainCollectionView.SourceCollection).Add(newUserAccount);
            return newUserAccount;
        }

        protected override void OnAddEntity(Party newUserAccount)
        {
            var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();
            if (serviceProviderContext == null) return;

            //Add the current user to the Administrator role of the ServiceProvider
            serviceProviderContext.OwnedRoles.First(r => r.Name == "Administrator").MemberParties.Add(newUserAccount);
        }

        protected override bool EntityIsPartOfView(Party entity, bool isNew)
        {
            if (isNew)
                return true;

            var entityIsPartOfView = true;

            //Setup filters

            var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();
            if (serviceProviderContext != null)
                entityIsPartOfView = serviceProviderContext.OwnedRoles.Any(r => r.MemberParties.Any(mp => mp != null && mp.Id == entity.Id));

            return entityIsPartOfView;
        }

        #endregion
    }
}

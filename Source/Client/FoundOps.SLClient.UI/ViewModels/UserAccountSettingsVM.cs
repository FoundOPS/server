using ReactiveUI;
using System.Reactive.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccountSettings
    /// </summary>
    [ExportViewModel("UserAccountSettingsVM")]
    public class UserAccountSettingsVM : CoreEntityVM
    {
        private readonly ObservableAsPropertyHelper<UserAccount> _selectedUserAccount;

        /// <summary>
        /// Gets or sets the selected user account.
        /// </summary>
        /// <value>
        /// The selected user account.
        /// </value>
        public UserAccount SelectedUserAccount { get { return _selectedUserAccount.Value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountSettingsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public UserAccountSettingsVM(DataManager dataManager)
            : base(dataManager)
        {
            _selectedUserAccount = dataManager.ContextManager.UserAccountObservable.SubscribeOnDispatcher().Select(ua => ua)
                 .ToProperty(this, x => x.SelectedUserAccount);

            //Update each user account to have a party image if it does not already
            //dataManager.ContextManager.UserAccountObservable.Delay(new TimeSpan(0, 0, 0, 2))
            //    .Where(ua => ua != null && ua.PartyImage == null).SubscribeOnDispatcher()
            //    .Subscribe(ua => ua.PartyImage = new PartyImage { OwnerParty = ua });
        }
    }
}

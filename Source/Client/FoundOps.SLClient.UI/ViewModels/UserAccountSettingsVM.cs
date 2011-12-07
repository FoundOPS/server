using ReactiveUI;
using System.Reactive.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
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
        }
    }
}

using System.Reactive.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the BusinessAccountSettings
    /// </summary>
    [ExportViewModel("BusinessAccountSettingsVM")]
    public class BusinessAccountSettingsVM : CoreEntityVM
    {
        private readonly ObservableAsPropertyHelper<BusinessAccount> _selectedBusinessAccount;
        /// <summary>
        /// Gets the selected business account.
        /// </summary>
        public BusinessAccount SelectedBusinessAccount { get { return _selectedBusinessAccount.Value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountSettingsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public BusinessAccountSettingsVM(DataManager dataManager)
            : base(dataManager)
        {
            _selectedBusinessAccount =dataManager.ContextManager.OwnerAccountObservable.SubscribeOnDispatcher().Select(ba => (BusinessAccount) ba)
                .ToProperty(this, x => x.SelectedBusinessAccount);
        }
    }
}

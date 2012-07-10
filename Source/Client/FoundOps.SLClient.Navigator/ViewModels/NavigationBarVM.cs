using FoundOps.Common.Silverlight.Loader;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace FoundOps.SLClient.Navigator.ViewModels
{
    /// <summary>
    /// Handles the navigation bars logic
    /// </summary>
    [ExportViewModel("NavigationBarVM")]
    public class NavigationBarVM : ThreadableVM
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the navigate to command.
        /// </summary>
        /// <value>
        /// The navigate to command.
        /// </value>
        public RelayCommand<string> NavigateToCommand { get; set; }

        #region Parties/Roles

        private UserAccount _currentUserAccount;
        /// <summary>
        /// Gets the current user account.
        /// </summary>
        public UserAccount CurrentUserAccount
        {
            get { return _currentUserAccount; }
            private set
            {
                _currentUserAccount = value;

                this.RaisePropertyChanged("CurrentUserAccount");

                if (CurrentUserAccount == null)
                    return;

                this.RaisePropertyChanged("OwnerAccountsOfMyRoles");
                SelectedOwnerAccountOfRole = OwnerAccountsOfMyRoles.FirstOrDefault();
                OnDataLoaded();
            }
        }

        /// <summary>
        /// Gets a list of owner accounts for the role.
        /// </summary>
        public IEnumerable<Party> OwnerAccountsOfMyRoles
        {
            get
            {
                if (CurrentUserAccount == null)
                    return null;

                var orderedParties = CurrentUserAccount.AccessibleRoles.Select(r => r.OwnerParty).Distinct().ToList();

                //Compare the names alphabetically
                orderedParties.Sort((a, b) => a.DisplayName.ToString().CompareTo(b.DisplayName.ToString()));

                //Currently the UserAccountRole should not be displayed
                //when that changes TODO: Remove the following line and uncomment the 3 lines below
                orderedParties.Remove(CurrentUserAccount);

                //TODO: Add the following 3 lines back
                ////If the CurrentUserAccount is in the list put it last
                //if (orderedParties.Remove(CurrentUserAccount))
                //    orderedParties.Add(CurrentUserAccount);

                SelectedOwnerAccountOfRole = orderedParties.FirstOrDefault();

                return orderedParties;
            }
        }

        private Party _selectedOwnerAccountOfRole;
        /// <summary>
        /// Gets or sets the selected owner account of role.
        /// </summary>
        /// <value>
        /// The selected owner account of role.
        /// </value>
        public Party SelectedOwnerAccountOfRole
        {
            get { return _selectedOwnerAccountOfRole; }
            set
            {
                _selectedOwnerAccountOfRole = value;

                //The SelectedRole will be the only Role the CurrentUser has access to that is owned by the SelectedOwnerAccountOfRole
                //You can only have one role per Party
                SelectedRole = this.CurrentUserAccount.AccessibleRoles.FirstOrDefault(r => r.OwnerPartyId == SelectedOwnerAccountOfRole.Id);

                //Whenever the OwnerAccount is a BusinessAccount load CompanyHome page
                if (SelectedOwnerAccountOfRole != null && SelectedOwnerAccountOfRole is BusinessAccount)
                    OnNavigateToHandler("CompanyHome");

                this.RaisePropertyChanged("SelectedOwnerAccountOfRole");
            }
        }

        private Role _selectedRole;
        /// <summary>
        /// Gets or sets the selected role.
        /// </summary>
        /// <value>
        /// The selected role.
        /// </value>
        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;

                this.RaisePropertyChanged("SelectedRole");

                //Update the ContextManager's RoleId
                Manager.Context.RoleIdObserver.OnNext(SelectedRole.Id);
            }
        }

        #endregion

        private bool _blockLoading;
        /// <summary>
        /// Gets or sets a value indicating whether [block loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [block loading]; otherwise, <c>false</c>.
        /// </value>
        public bool BlockLoading
        {
            get { return _blockLoading; }
            set
            {
                _blockLoading = value;
                this.RaisePropertyChanged("BlockLoading");
            }
        }

        /// <summary>
        /// Gets or sets the content of the selected block.
        /// </summary>
        /// <value>
        /// The content of the selected block.
        /// </value>
        public object SelectedContent { get; set; }

        #endregion

        #region Local Variables

        private bool _setupBlocksMappingInMEFContentLoader;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationBarVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public NavigationBarVM()
        {
            Manager.Context.UserAccountObservable.Subscribe(ua => CurrentUserAccount = ua);
            OnDataLoaded();
        }

        protected override void RegisterCommands()
        {
            NavigateToCommand = new RelayCommand<string>(OnNavigateToHandler, uri => CurrentUserAccount != null && _setupBlocksMappingInMEFContentLoader);
        }

        protected override void RegisterMessages()
        {
        }

        #endregion

        #region Logic

        private bool _firstBlockChosen;
        /// <summary>
        /// Tracks chosen block analytics
        /// </summary>
        public void SetSelectedBlock(Block block)
        {
            var blockName = block.Name;

            Section section;
            if (blockName == "Feedback and Support")
                section = Section.FeedbackAndSupport;
            else if (blockName == "Business Accounts")
                section = Section.BusinessAccounts;
            else
                section = (Section)Enum.Parse(typeof(Section), blockName, true);

            if (!_firstBlockChosen)
            {
                Data.Services.Analytics.Track(Event.FirstSectionChosen, section);
                _firstBlockChosen = true;
            }

            Data.Services.Analytics.Track(Event.SectionChosen, section);
        }

        /// <summary>
        /// Called when [navigate to handler].
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void OnNavigateToHandler(String uri)
        {
            //If the SelectedContent's DataContext (usually a VM) is IPreventNavigationFrom. Check if you can navigate
            if (SelectedContent != null)
            {
                var selectedContentDataContext = ((FrameworkElement)SelectedContent).DataContext;
                var preventNavigationFrom = selectedContentDataContext as IPreventNavigationFrom;

                if (preventNavigationFrom != null)
                {
                    if (!preventNavigationFrom.CanNavigateFrom(() => MessageBus.Current.SendMessage(new NavigateToMessage { UriToNavigateTo = new Uri(uri, UriKind.RelativeOrAbsolute) })))
                        return;

                    (preventNavigationFrom).OnNavigateFrom();
                }
            }

            var uriStringWithParameters = uri;

            bool firstParameter = true;

            if (SelectedRole != null)
            {
                firstParameter = false;
                uriStringWithParameters = String.Format("{0}?roleid={1}", uriStringWithParameters, SelectedRole.Id);
            }

            if (SelectedOwnerAccountOfRole != null)
            {
                if (firstParameter)
                {
                    uriStringWithParameters = String.Format("{0}?roleowneraccountid={1}", uriStringWithParameters, SelectedOwnerAccountOfRole.Id);
                    firstParameter = false;
                }
                else
                    uriStringWithParameters = String.Format("{0}&roleowneraccountid={1}", uriStringWithParameters, SelectedOwnerAccountOfRole.Id);
            }

            MessageBus.Current.SendMessage(new NavigateToMessage { UriToNavigateTo = new Uri(uriStringWithParameters, UriKind.RelativeOrAbsolute) });
        }

        private void OnDataLoaded()
        {
            if (CurrentUserAccount == null) return;

            SetupBlocksMappingInMEFContentLoader();
            NavigateToCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Setups the blocks mapping in MEF content loader.
        /// </summary>
        private void SetupBlocksMappingInMEFContentLoader()
        {
            //TODO
            //foreach (var block in CurrentUserAccount.AccessibleBlocks.Where(block => block.Link != null))
            //    MEFBlockLoader.MapUri(new Uri(block.NavigateUri, UriKind.RelativeOrAbsolute), new Uri(block.Link, UriKind.RelativeOrAbsolute));

            _setupBlocksMappingInMEFContentLoader = true;
        }

        #endregion
    }
}

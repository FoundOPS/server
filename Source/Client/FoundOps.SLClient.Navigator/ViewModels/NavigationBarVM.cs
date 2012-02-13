using System;
using ReactiveUI;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.Common.Silverlight.Loader;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Common.Silverlight.MVVM.Interfaces;

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

        private IEnumerable<Block> _publicBlocks;
        /// <summary>
        /// Gets the public blocks.
        /// </summary>
        public IEnumerable<Block> PublicBlocks
        {
            get { return _publicBlocks; }
            private set
            {
                _publicBlocks = value;
                this.RaisePropertyChanged("PublicBlocks");
            }
        }

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

                #region QuickBooks Stuff

                //var isAuthorizedUri = string.Concat("http://localhost:31820/Quickbooks/NeedsAuthorization", "?roleId=", SelectedRole.Id.ToString());

                //var client = new WebClient();
                ////Check if the current BusinessAccount needs QuickBooks authorization
                //client.DownloadStringCompleted += (sender, e) =>
                //{
                //    //If it needs QuickBooks authorization, get it in a new window
                //    if (e.Result.ToLower() == "true")
                //    {
                //        //creates the uri for the popup mentioned above
                //        var uriString = string.Concat("http://localhost:31820/Quickbooks/GetAuthorization", "?roleId=", SelectedRole.Id.ToString());

                //        //generates the Html popup using the uri and options
                //        HtmlPage.Window.Navigate(new Uri(uriString), "new");
                //    }
                //};

                //client.DownloadStringAsync(new Uri(isAuthorizedUri));

                #endregion

                this.RaisePropertyChanged("SelectedRole");

                //Update the ContextManager's RoleId
                _dataManager.ContextManager.RoleIdObserver.OnNext(SelectedRole.Id);
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

        private bool _aBlockHasBeenClicked;
        /// <summary>
        /// Tracks the first block that was clicked (for analytics)
        /// </summary>
        public Block BlockClickedOn
        {
            set
            {
                if (_aBlockHasBeenClicked)
                    return;

                _aBlockHasBeenClicked = true;

                //Trigger analytic here
                Data.Services.Analytics.FirstBlockNavigatedTo(value.Name);
            }
        }

        /// <summary>
        /// Gets or sets the content of the selected block.
        /// </summary>
        /// <value>
        /// The content of the selected block.
        /// </value>
        public object SelectedContent { get; set; }

        private readonly DataManager _dataManager;
        /// <summary>
        /// Gets the data manager.
        /// </summary>
        public DataManager DataManager
        {
            get { return _dataManager; }
        }

        #endregion

        #region Local Variables

        private bool _setupBlocksMappingInMEFContentLoader;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationBarVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public NavigationBarVM(DataManager dataManager)
        {
            _dataManager = dataManager;

            DataManager.ContextManager.UserAccountObservable.Subscribe(ua => CurrentUserAccount = ua);

            dataManager.GetPublicBlocks(blocks =>
            {
                PublicBlocks = blocks;
                OnDataLoaded();
            });
        }

        #region NavigationBarVM's Logic

        protected override void RegisterCommands()
        {
            NavigateToCommand = new RelayCommand<string>(OnNavigateToHandler, uri => CurrentUserAccount != null && _setupBlocksMappingInMEFContentLoader);
        }

        protected override void RegisterMessages()
        {
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
            foreach (var block in CurrentUserAccount.AccessibleBlocks.Union(PublicBlocks).Where(block => block.Link != null))
                MEFBlockLoader.MapUri(new Uri(block.NavigateUri, UriKind.RelativeOrAbsolute), new Uri(block.Link, UriKind.RelativeOrAbsolute));

            _setupBlocksMappingInMEFContentLoader = true;
        }

        #endregion
    }
}

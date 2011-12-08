using System;
using ReactiveUI;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using FoundOps.Core.Navigator.VMs;
using FoundOps.Core.Navigator.Loader;
using FoundOps.SLClient.Navigator.ViewModels;
using FoundOps.Common.Silverlight.MVVM.Messages;

namespace FoundOps.SLClient.Navigator
{
    /// <summary>
    /// The MainPage of the entire Silverlight Application
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            MessageBus.Current.Listen<NavigateToMessage>().Subscribe(OnNavigateToMessageRecieved);
            MessageBus.Current.Listen<BlockMappingSetupMessage>().Subscribe(OnBlockMappingSetupMessageRecieved);

            //Automatically size the ContentFrame's content to the size of the ContentFrame
            //This is especially important for Dispatcher
            Application.Current.Host.Content.Resized += ContentResized;
        }

        #region ContentFrame.Content's Sizing

        void ContentResized(object sender, EventArgs e)
        {
            SetContentPageSizeToContentFrameSize();
        }

        private void SetContentPageSizeToContentFrameSize()
        {
            if (ContentFrame.Content as FrameworkElement == null) return;

            ((FrameworkElement)ContentFrame.Content).Width = ContentFrame.ActualWidth;
            ((FrameworkElement)ContentFrame.Content).Height = ContentFrame.ActualHeight;
        }

        #endregion

        void ContentFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            SetContentPageSizeToContentFrameSize();

            var navigationContext = ((Page)ContentFrame.Content).NavigationContext;

            //Update the RoleId
            if (navigationContext.QueryString.ContainsKey("roleid"))
            {
                var roleId = new Guid(navigationContext.QueryString["roleid"]);

                ((NavigationBarVM)this.DataContext).DataManager.ContextManager.RoleIdObserver.OnNext(roleId);
            }
        }

        private void OnBlockMappingSetupMessageRecieved(BlockMappingSetupMessage obj)
        {
            var navigationState = Application.Current.Host.NavigationState;

            if (!String.IsNullOrEmpty(navigationState)) //Navigate to navigation state if it exists (after the Block Mapping is setup)
            {
                ContentFrame.Navigate(new Uri(navigationState, UriKind.RelativeOrAbsolute));
            }
        }

        private void OnNavigateToMessageRecieved(NavigateToMessage navigateToMessage)
        {
            ContentFrame.Navigate(navigateToMessage.UriToNavigateTo);
        }

        private void MefBlockLoaderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
                ((NavigationBarVM)this.DataContext).BlockLoading = ((MEFBlockLoader)this.Resources["MEFContentLoader"]).IsBusy;
        }

        private void CompassImageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
#if DEBUG
                HtmlPage.Window.Navigate(new Uri("http://localhost:31820/Home/Silverlight"));
#else
                HtmlPage.Window.Navigate(new Uri("http://www.foundops.com/Home/Silverlight"));
#endif
        }
    }
}

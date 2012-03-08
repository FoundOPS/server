using System;
using FoundOps.Common.Tools.ExtensionMethods;
using ReactiveUI;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using FoundOps.Common.Silverlight.Loader;
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
            if (!navigationContext.QueryString.ContainsKey("roleid")) return;

            var roleId = new Guid(navigationContext.QueryString["roleid"]);
            ((NavigationBarVM)this.DataContext).DataManager.ContextManager.RoleIdObserver.OnNext(roleId);
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
                HtmlPage.Window.Navigate(new Uri(UriExtensions.ThisRootUrl + "/Home/Silverlight"));
        }
    }
}

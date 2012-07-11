using FoundOps.Common.Tools;
using FoundOps.SLClient.UI.Tools;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Browser;

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

            //Automatically size the ContentFrame's content to the size of the ContentFrame
            //This is especially important for Dispatcher
            Application.Current.Host.Content.Resized += ContentResized;

            this.Loaded += MainPageLoaded;

            VM.Navigation.FromPropertyChanged("SelectedView").ObserveOnDispatcher().Subscribe(_ => SetContentPageSizeToContentFrameSize());
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            HtmlPage.RegisterScriptableObject("navigationVM", VM.Navigation);
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
    }
}

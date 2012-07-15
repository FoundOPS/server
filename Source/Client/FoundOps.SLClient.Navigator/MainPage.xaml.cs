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
        //Whenever the application is clicked this will fire
        //Expose this for the navigator
        [ScriptableMemberAttribute]
        public event EventHandler Clicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            //Automatically size the ContentFrame's content to the size of the ContentFrame
            //This is especially important for Dispatcher
            Application.Current.Host.Content.Resized += ContentResized;

            this.MouseLeftButtonDown += (s, e) =>
            {
                if (Clicked != null)
                    Clicked(s, e);
            };

            this.Loaded += MainPageLoaded;

            VM.Navigation.FromPropertyChanged("SelectedView").ObserveOnDispatcher().Subscribe(_ => SetContentPageSizeToContentFrameSize());
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            //For the click event, expost this
            HtmlPage.RegisterScriptableObject("mainPage", this);

            //For navigation, expose the navigationVM
            HtmlPage.RegisterScriptableObject("navigationVM", VM.Navigation);
        }

        #region ContentFrame.Content's Sizing

        void ContentResized(object sender, EventArgs e)
        {
            SetContentPageSizeToContentFrameSize();
        }

        private void SetContentPageSizeToContentFrameSize()
        {
            if (this.Content as FrameworkElement == null) return;

            ((FrameworkElement)this.Content).Width = this.ActualWidth;
            ((FrameworkElement)this.Content).Height = this.ActualHeight;
        }

        #endregion
    }
}

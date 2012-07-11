using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Navigator.Panes.InfiniteAccordion;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

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

            NavigationVM.FromPropertyChanged("SelectedView").ObserveOnDispatcher().Subscribe(_ => SetContentPageSizeToContentFrameSize());

            NavigationVM.NavigateToView("Business Accounts");
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

        private static NavigationVM NavigationVM
        {
            get
            {
                return (NavigationVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("NavigationVM", null, CreationPolicy.Shared).Value;
            }
        }
    }
}

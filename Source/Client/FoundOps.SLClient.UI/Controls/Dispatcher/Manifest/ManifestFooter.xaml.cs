using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Documents;
using Telerik.Windows.Documents.UI;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// The footer for a route manifest.
    /// </summary>
    public partial class ManifestFooter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestFooter"/> class.
        /// </summary>
        public ManifestFooter()
        {
            InitializeComponent();
        }

        private void CurrentPageBlockLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is DocumentPagePresenter))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent == null) return;
            var presenter = (DocumentPagePresenter)parent;
            int pageNumber = presenter.SectionBoxIndex + 1;
            this.CurrentPageBlock.Text = pageNumber.ToString();
        }

        private void PageCountBlockLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is DocumentPagePresenter))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent == null) return;
            var presenter = (DocumentPagePresenter)parent;
            var position = new DocumentPosition(presenter.Owner.Document);
            position.MoveToLastPositionInDocument();

            this.CurrentPageBlock.Text = (position.GetCurrentSectionBox().ChildIndex + 1).ToString();
        }
    }
}
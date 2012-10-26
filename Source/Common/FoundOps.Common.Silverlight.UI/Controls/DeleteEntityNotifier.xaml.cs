using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class DeleteEntityNotifier
    {
        public DeleteEntityNotifier(string recurringServiceCount, string futureServiceCount, string entityType)
        {
            InitializeComponent();
            this.EntityTypeTextBlock.Text = entityType;
            this.RecurringServiceCountTextBlock.Text = recurringServiceCount;
            this.FutureServiceCountTextBox.Text = futureServiceCount;
        }

        public DeleteEntityNotifier(string recurringServiceCount, string futureServiceCount, string locationCount, string entityType) : this(recurringServiceCount, futureServiceCount, entityType)
        {
            this.LocationStackPanel.Visibility = Visibility.Visible;
            this.LocationCountTextBlock.Text = locationCount;
        }

        public Button ContinueButton
        {
            get { return Continue; }
        }
        public Button CancelButton
        {
            get { return Cancel; }
        }
    }
}


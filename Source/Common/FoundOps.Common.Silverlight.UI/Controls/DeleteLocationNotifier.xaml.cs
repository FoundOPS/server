using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class DeleteLocationNotifier
    {
        public DeleteLocationNotifier(string recurringServiceCount, string futureServiceCount)
        {
            InitializeComponent();
            this.RecurringServiceCountTextBlock.Text = recurringServiceCount;
            this.FutureServiceCountTextBox.Text = futureServiceCount;
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


using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.Controls
{
    public partial class RemoveDeleteCancel
    {
        public RemoveDeleteCancel()
        {
            InitializeComponent();
        }

        public Button RemoveButton
        {
            get { return Remove; }
        }

        public Button DeleteButton
        {
            get { return Delete; }
        }

        public Button CancelButtonRDC
        {
            get { return Cancel; }
        }

        public string LocationNameTextBox
        {
            set { this.LocationName.Text = value; }
        }

        public string ClientNameTextBox
        {
            set { this.ClientName.Text = value; }
        }
    }
}


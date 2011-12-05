using System.Windows;

namespace FoundOps.Framework.Views.Controls.Services
{
    public partial class InstanceEditChildWindow
    {
        public InstanceEditChildWindow()
        {
            InitializeComponent();
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


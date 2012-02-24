using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class ErrorWindow
    {
        public ErrorWindow()
        {
            InitializeComponent();
            this.Title = "Whoops!";
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


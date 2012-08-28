using System.Windows;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class LocationErrorWindow
    {
        public LocationErrorWindow()
        {
            InitializeComponent();
            this.Title = "Whoops!";
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.Blocks;
using FoundOps.Common.Silverlight.Models.Import;

namespace FoundOps.SLClient.Navigator.Panes
{
    /// <summary>
    /// UI for importing data into the system.
    /// </summary>
    [ExportPage("ImportData")]
    public partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        private void SelectFileButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = "CSV Files|*.csv"};
            var userClickedOk = openFileDialog.ShowDialog();
            if (userClickedOk == true)
                ((ImportDataVM) this.DataContext).SelectFileCommand.Execute(openFileDialog.File);
        }
    }

    public class IndexToImportDestinationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ImportDestination)value;
        }
    }
}

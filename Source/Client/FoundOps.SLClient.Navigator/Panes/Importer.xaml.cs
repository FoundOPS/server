using FoundOps.Core.Models.Import;
using FoundOps.SLClient.UI.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;

namespace FoundOps.SLClient.Navigator.Panes
{
    /// <summary>
    /// UI for importing data into the system.
    /// </summary>
    [Export("Importer")]
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

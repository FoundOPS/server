using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FoundOps.Common.Silverlight.Blocks;
using FoundOps.Framework.Views.Models;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.Navigator.Panes
{
    [ExportPage("ImportData")]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void SelectFileButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files|*.csv";
            var userClickedOK = openFileDialog.ShowDialog();
            if (userClickedOK == true)
            {
                ((ImportDataVM) this.DataContext).SelectFileCommand.Execute(openFileDialog.File);
            }
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

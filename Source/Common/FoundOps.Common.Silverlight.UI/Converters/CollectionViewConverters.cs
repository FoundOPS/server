using System;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;

namespace FoundOps.Common.Silverlight.Converters
{
    public class OrderByParameterConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortBy = parameter as String ?? "";

            var viewSource = new CollectionViewSource { Source = value };
            viewSource.SortDescriptions.Add(new SortDescription(sortBy, ListSortDirection.Ascending));
            return viewSource.View;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

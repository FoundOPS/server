using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FoundOps.Core.Views.Converters
{
    public class OrderByTypeLabelConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewSource = new CollectionViewSource { Source = value };
            viewSource.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            viewSource.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
            return viewSource.View;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "Email Address")
            {
                const string source = "/FoundOps.Common.Silverlight;component/Resources/EmailSymbol.png";
                return source;
            }
            if (value.ToString() == "Website")
            {
                const string source = "/FoundOps.Common.Silverlight;component/Resources/WebsiteSymbol.png";
                return source;
            }
            if (value.ToString() == "Phone Number")
            {
                const string source = "/FoundOps.Common.Silverlight;component/Resources/PhoneSymbol.png";
                return source;
            }
            if (value.ToString() == "Fax Number")
            {
                const string source = "/FoundOps.Common.Silverlight;component/Resources/FaxSymbol.png";
                return source;
            }

            return new BitmapImage();
              
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

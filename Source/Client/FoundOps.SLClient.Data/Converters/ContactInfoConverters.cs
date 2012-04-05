using FoundOps.Core.Models.CoreEntities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// Orders contact info first by Label then by Type.
    /// </summary>
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

    /// <summary>
    /// Selects the proper icon for a contact info's type.
    /// </summary>
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

    /// <summary>
    /// Selects the proper datatemplate depending on the contact info type.
    /// </summary>
    public class ContactInfoDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string type = ((ContactInfo)item).Type;

            switch (type)
            {
                case "Email Address":
                    return EmailAddressTemplate;
                case "Phone Number":
                    return PhoneNumberTemplate;
                case "Website":
                    return WebsiteTemplate;
                default:
                    return OtherTemplate;
            }
        }

        public DataTemplate EmailAddressTemplate { get; set; }
        public DataTemplate PhoneNumberTemplate { get; set; }
        public DataTemplate WebsiteTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }
    }
}

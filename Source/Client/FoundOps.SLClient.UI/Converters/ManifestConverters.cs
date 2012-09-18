using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Allows manifest to only show the location name
    /// if it's different than the client name.
    /// </summary>
    public class LocationNameVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var clientName = values[0] as string;
            var locationName = values[1] as string;

            if (clientName == null || locationName == null)
                return Visibility.Visible;

            return clientName != locationName ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

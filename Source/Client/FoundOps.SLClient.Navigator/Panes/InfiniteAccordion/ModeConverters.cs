using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.Navigator.Panes.InfiniteAccordion
{
    /// <summary>
    /// A converter to choose DetailsOnly or ListDetailsSummary for the ServiceTemplatesThirdsListDetails
    /// </summary>
    public class ServiceTemplatesModeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return ThirdsListDetailsMode.DetailsOnly;

            var currentContext = (ObservableCollection<object>) value;

            if (currentContext.Count == 0)
                return ThirdsListDetailsMode.ListDetailsSummary;

            return ThirdsListDetailsMode.DetailsOnly;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

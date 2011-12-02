using System;
using System.Globalization;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Selectors
{
    /// <summary>
    /// A template selector for entitys. 
    /// If the Entity is new it will return EditableTemplate.
    /// If the Entity is existing it will return ReadOnlyTemplate.
    /// </summary>
    public class EntityTemplateSelector : IValueConverter
    {
        /// <summary>
        /// Use this template whenever the Entity is new.
        /// </summary>
        public DataTemplate NewItemTemplate { get; set; }

        /// <summary>
        /// Use this template whenever the Entity is existing.
        /// </summary>
        public DataTemplate ExistingItemTemplate { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var entity = (Entity)value;

            return entity.EntityState == EntityState.New ? NewItemTemplate : ExistingItemTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

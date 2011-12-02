using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using System.Collections.Generic;

namespace FoundOps.Common.Silverlight.Tools
{
    public class DisableIfInItemsSourceStyleSelector : StyleSelector
    {
        public Style EnabledStyle { get; set; }
        public Style DisabledStyle { get; set; }

        public IEnumerable<object> ItemsSource { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item == null || ItemsSource == null)
                return EnabledStyle;

            return ItemsSource.Any(i => i == item) ? DisabledStyle : EnabledStyle;
        }

    }
}

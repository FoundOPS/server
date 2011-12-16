using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Windows.Controls.Primitives;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    /// <summary>
    /// A RadioButton column that only allows one item to be set.
    /// </summary>
    public class EditRadioButtonColumn : GridViewDataColumn
    {
        public override FrameworkElement CreateCellElement(GridViewCell cell, object dataItem)
        {
            var radioButton = new RadioButton { Margin = new Thickness(3.0, 0.0, 3.0, 0.0) };

            if (DataMemberBinding != null)
                radioButton.SetBinding(ToggleButton.IsCheckedProperty, DataMemberBinding);

            radioButton.Checked += RadioButtonChecked;

            return radioButton;
        }

        void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var currentRadioButton = (RadioButton)sender;

            if (this.DataControl == null)
                return;

            this.DeselectAllOtherItems(currentRadioButton);
        }

        private void DeselectAllOtherItems(RadioButton currentRadioButton)
        {
            if (currentRadioButton.IsChecked != true) return;

            var children = this.DataControl.ChildrenOfType<RadioButton>();

            foreach (var radioButton in children.Where(rb => rb != currentRadioButton))
                radioButton.IsChecked = false;
        }
    }
}

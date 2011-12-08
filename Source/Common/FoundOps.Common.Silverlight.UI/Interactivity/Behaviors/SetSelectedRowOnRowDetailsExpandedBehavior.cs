using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.Interactivity.Behaviors
{
    public class SetSelectedRowOnRowDetailsExpandedBehavior : Behavior<RadGridView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RowDetailsVisibilityChanged += AssociatedObject_RowDetailsVisibilityChanged;
        }

        void AssociatedObject_RowDetailsVisibilityChanged(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            //Select this row, that way SelectedUser is updated properly
            if (e.Visibility.HasValue && e.Visibility.Value == Visibility.Visible)
                this.AssociatedObject.SelectedItem = e.Row.DataContext;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RowDetailsVisibilityChanged -= AssociatedObject_RowDetailsVisibilityChanged;
        }
    }
}

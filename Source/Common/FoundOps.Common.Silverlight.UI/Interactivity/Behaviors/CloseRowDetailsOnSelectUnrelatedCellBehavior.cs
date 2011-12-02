using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace ScalableCourier.Client.CommonResources.Interactivity.Behaviors
{
    public class CloseRowDetailsOnSelectUnrelatedCellBehavior : Behavior<RadGridView>
    {
        private ObservableCollection<GridViewRow> _rowsWithDetailsVisible = new ObservableCollection<GridViewRow>();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RowDetailsVisibilityChanged += new EventHandler<Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs>(AssociatedObject_RowDetailsVisibilityChanged);
            this.AssociatedObject.SelectionChanged += new EventHandler<SelectionChangeEventArgs>(AssociatedObject_SelectionChanged);
        }

        void AssociatedObject_RowDetailsVisibilityChanged(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            if (e.Row.DetailsVisibility == Visibility.Visible)
                _rowsWithDetailsVisible.Add(e.Row);
            else
                _rowsWithDetailsVisible.Remove(e.Row);
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var rowsToRemove = new ObservableCollection<GridViewRow>(); 
            foreach (var rowWithDetailsVisible in _rowsWithDetailsVisible)
            {
                if (!this.AssociatedObject.SelectedItems.Contains(rowWithDetailsVisible.Item)) //If there is a row with details visible check to make sure it is still selected
                    rowsToRemove.Add(rowWithDetailsVisible); //Close the row it if it is not still selected
            }
            foreach (var row in rowsToRemove) //Seperated out removing rows into other foreach loop so as not crash first foreach loop
                row.DetailsVisibility = Visibility.Collapsed; 
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RowDetailsVisibilityChanged -= new EventHandler<Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs>(AssociatedObject_RowDetailsVisibilityChanged);
            this.AssociatedObject.SelectionChanged -= new EventHandler<SelectionChangeEventArgs>(AssociatedObject_SelectionChanged);
        }
    }
}

using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods
{
    public static class GridViewExtensions
    {
        public static GridViewCell GetCellByContent(this RadGridView gridView, object cellValue)
        {
            return
                (from cell in gridView.ChildrenOfType<GridViewCell>()
                 where cell.Value.ToString() == cellValue.ToString()
                 select cell).FirstOrDefault();
        }

        public static GridViewCell GetCellByIndexes(this RadGridView gridView, int rowIndex, int columnIndex)
        {
            return
                (from cell in gridView.ChildrenOfType<GridViewCell>()
                 where gridView.Columns.IndexOf(cell.Column) == columnIndex
                 select cell).Skip(rowIndex).FirstOrDefault();

        }
    }
}

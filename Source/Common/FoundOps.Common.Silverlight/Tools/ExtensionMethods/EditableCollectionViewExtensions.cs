using System.ComponentModel;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    public static class EditableCollectionViewExtensions
    {
        public static void Commit(this IEditableCollectionView view)
        {
            if (view.IsAddingNew)
                view.CommitNew();
            if (view.IsEditingItem)
                view.CommitEdit();
        }
    }
}

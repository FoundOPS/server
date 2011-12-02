using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.Common.Silverlight.MVVM.Models
{
    public static class DomainCollectionViewExtensions
    {
        public static void Commit(this DomainCollectionView view)
        {
            if (view.IsAddingNew)
                view.CommitNew();
            if (view.IsEditingItem)
                view.CommitEdit();
        }
    }
}

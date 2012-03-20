using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class BackedVirtualQueryableCollectionView<T> : VirtualQueryableCollectionView<T>
    {
        public bool LoadingVirtualItems { get; set; }

        public bool UpdatingCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">The collection to follow changes from.</param>
        public BackedVirtualQueryableCollectionView(ObservableCollection<T> source, Func<int> countQuery, Func<int, object> loadItemsQuery)
        {
            
        }
    }
}

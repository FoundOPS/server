using System.Windows;
using System.Collections.ObjectModel;

namespace FoundOps.Common.Silverlight.UI.Controls.Printing
{
    /// <summary>
    /// A paged printer.
    /// </summary>
    public interface IPagedPrinter
    {
        /// <summary>
        /// Gets the pages.
        /// </summary>
        ObservableCollection<FrameworkElement> Pages { get; }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Prints this instance.
        /// </summary>
        void Print();
    }
}

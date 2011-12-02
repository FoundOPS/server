using System.Windows;

namespace FoundOps.Common.Silverlight.Tools.Printing
{
    public interface IPagedPrinter
    {
        int PageCount { get; }
        int CurrentPageIndex { get; set; }
        bool IsLastPage { get; }
        bool IsFirstPage { get; }
        void Print();
    }
}

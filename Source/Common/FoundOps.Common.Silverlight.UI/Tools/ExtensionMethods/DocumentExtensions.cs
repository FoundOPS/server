using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Layout;

namespace FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods
{
    public static class DocumentExtensions
    {
        public static int GetPageNumberForDocumentElement(this DocumentElement element)
        {
            var layoutBox = element.FirstLayoutBox;
            while (!(layoutBox is SectionLayoutBox))
            {
                layoutBox = layoutBox.Parent;
            }
            return layoutBox.ChildIndex;
        }
    }
}

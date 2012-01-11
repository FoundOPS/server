namespace FoundOps.Common.Silverlight.UI.Controls.Printing
{
    /// <summary>
    /// A paged viewer.
    /// </summary>
    public interface IPagedViewer
    {
        /// <summary>
        /// Gets or sets the index of the current page.
        /// </summary>
        /// <value>
        /// The index of the current page.
        /// </value>
        int PageIndex { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is on the first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        bool IsFirstPage { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is on the last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        bool IsLastPage { get; }
    }
}
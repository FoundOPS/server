using FoundOps.Common.Silverlight.Models.Import;
using Telerik.Data;

namespace FoundOps.SLClient.UI.Controls.ImportData
{
    /// <summary>
    /// A DataColumn that has an ImportColumnType.
    /// </summary>
    public class ImportColumn : DataColumn
    {
        /// <summary>
        /// Gets or sets the type of the import column.
        /// </summary>
        /// <value>
        /// The type of the import column.
        /// </value>
        public ImportColumnType ImportColumnType { get; set; }
    }
}

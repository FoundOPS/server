namespace FoundOPS.API.Models
{
    /// <summary>
    /// An individual column
    /// </summary>
    public class Column
    {
        /// <summary>
        /// The field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If the column is hidden or not
        /// </summary>
        public string Hidden { get; set; }

        /// <summary>
        /// The column width
        /// </summary>
        public double Width { get; set; }
    }
}
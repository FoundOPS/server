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
        public bool Hidden { get; set; }

        /// <summary>
        /// The column width (a string so that px or % can be used)
        /// </summary>
        public string Width { get; set; }
    }
}
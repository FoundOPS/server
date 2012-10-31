namespace FoundOPS.API.Models
{
    public class Cell
    {
        /// <summary>
        /// The Row that the cell is in
        /// </summary>
        public int R { get; set; }

        /// <summary>
        /// The Column that the cell is in
        /// </summary>
        public int C { get; set; }

        /// <summary>
        /// The Value of the cell
        /// </summary>
        public string V { get; set; }
    }
}
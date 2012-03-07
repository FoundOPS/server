namespace FoundOps.Core.Models.Import
{
    /// <summary>
    /// Not in use yet.
    /// </summary>
    public enum Multiplicity
    {
        /// <summary>
        /// Can only have one piece of this information
        /// </summary>
        Single,
        /// <summary>
        /// Can have multiple pieces of information
        /// </summary>
        Multiple
    }

    /// <summary>
    /// Contains information about the DataColumn used to import a DataCategory.
    /// </summary>
    public class ImportColumnType
    {
        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// Gets the datacategory being imported.
        /// </summary>
        public DataCategory Type { get; private set; }

        /// <summary>
        /// Gets the multiplicity of the datacategory. NOTE: Currently everything works as a single association.
        /// </summary>
        public Multiplicity Multiplicity { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportColumnType"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="type">The type.</param>
        /// <param name="multiplicity">The multiplicity.</param>
        public ImportColumnType(string displayName, DataCategory type, Multiplicity multiplicity)
        {
            DisplayName = displayName;
            Type = type;
            Multiplicity = multiplicity;
        }
    }
}
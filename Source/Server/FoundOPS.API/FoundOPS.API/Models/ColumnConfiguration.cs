using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class ColumnConfiguration
    {
        /// <summary>
        /// The role the column configuration is for.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// The section the columns are from.
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Optional extra information to store about the configuration.
        /// </summary>
        public string Tag { get; set; }

        public Dictionary<Guid, Column[]> Columns { get; private set; }

        public ColumnConfiguration()
        {
            Columns = new Dictionary<Guid, Column[]>();
        }
    }
}
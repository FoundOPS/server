using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class ColumnConfiguration
    {
        /// <summary>
        /// A unique Id for finding the column configuration.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The role the column configuration is for.
        /// </summary>
        public Guid RoleId { get; set; }

        public List<Column> Columns { get; set; }

        public ColumnConfiguration()
        {
            Columns = new List<Column>();
        }
    }
}
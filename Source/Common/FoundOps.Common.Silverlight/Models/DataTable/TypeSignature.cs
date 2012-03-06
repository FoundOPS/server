using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data
{
    internal class TypeSignature : IEquatable<TypeSignature>
    {
        private readonly int hashCode;

        public TypeSignature(IEnumerable<DataColumn> columns)
        {
            this.hashCode = 0;
            foreach (var column in columns.OrderBy(p => p.ColumnName))
            {
                this.hashCode ^= column.ColumnName.GetHashCode() ^ column.DataType.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            return ((obj is TypeSignature) && this.Equals((TypeSignature) obj));
        }

        public bool Equals(TypeSignature other)
        {
            return this.hashCode.Equals(other.hashCode);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}

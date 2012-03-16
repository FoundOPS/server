using System;

namespace Telerik.Data
{
    public class DataRow
    {
        private readonly DataTable owner;
        private DynamicObject rowObject;

        protected internal DataRow(DataTable owner)
        {
            this.owner = owner;
        }

        public object this[string columnName]
        {
            get
            {
                return this.RowObject.GetValue<object>(columnName);
            }
            set
            {
                this.RowObject.SetValue(columnName, value);
            }
        }

        internal DynamicObject RowObject
        {
            get
            {
                this.EnsureRowObject();
                return this.rowObject; 
            }
        }

        private void EnsureRowObject()
        {
            if (this.rowObject == null)
                this.rowObject = (DynamicObject) Activator.CreateInstance(this.owner.ElementType);
        }
    }
}

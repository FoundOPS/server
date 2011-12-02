using System;

namespace FoundOps.Core.Models.CoreEntities
{
    public enum DateTimeType
    {
        DateTime = 0,
        TimeOnly = 1,
        DateOnly = 2
    }

    public partial class DateTimeField
    {
        public DateTimeType DateTimeType
        {
            get
            {
                return (DateTimeType)Convert.ToInt32(this.TypeInt);
            }
            set
            {
                this.TypeInt = Convert.ToInt16(value);
            }
        }

        partial void OnTypeIntChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("DateTimeType");
        }
    }
}
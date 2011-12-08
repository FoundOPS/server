using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Models.DataTable
{
	public class DataRow<T>
	{
        public Dictionary<string, T> Values { get; set; }

        public DataRow()
        {
            Values = new Dictionary<string, T>();
        }

	    public T this[string columnName]
        {
            get
            {
                return this.Values[columnName];
            }
            set
            {
                if (Values.ContainsKey(columnName))
                    this.Values[columnName] = value;
                else
                    Values.Add(columnName, value);
            }
        }
	}

    public class DataRowColumnToValueConverter<T>:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var dataRow = (DataRow<T>) value;
            var valueToReturn = dataRow[(string) parameter];
            return valueToReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

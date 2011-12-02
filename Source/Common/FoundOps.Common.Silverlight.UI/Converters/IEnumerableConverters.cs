using System;
using System.Collections;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class EnumerableOfTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Paramater should be an object of the type to extract from the IEnumerable
            //The following method will return value.OfType<parameter>()

            //Example:
            //Parameter is a Student, value is an IEnumerable<Person>(), (Student:Person)
            //This will return value.OfType<Student>();

            var typeToGetInEnumerable = parameter.GetType();
            var enumerable = (IEnumerable) value;
            //In example currentEnumerableType will be Person
            var ofTypeMethodInfo = typeof(System.Linq.Enumerable).GetMethod("OfType");

            var genericOfTypeMethodInfo = ofTypeMethodInfo.MakeGenericMethod(typeToGetInEnumerable);
            
            //In example will do value.OfType<Student>(); through reflection
            var result = genericOfTypeMethodInfo.Invoke(null, new object[] { enumerable });
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

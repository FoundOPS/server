using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
{
    public class RouteLogVehiclesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object item, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (item as IEnumerable<Vehicle> == null)
                return "";

            var vehicles = (IEnumerable<Vehicle>) item;
            if(vehicles.Count()<=0)
                return "";

            var vehiclesString = vehicles.First().VehicleId;

            return vehicles.Skip(1).Aggregate(vehiclesString, (current, vehicle) => current + (", " + vehicle.VehicleId));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RouteLogTechniciansConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object item, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (item as IEnumerable<Employee> == null)
                return "";

            var employees = (IEnumerable<Employee>)item;
            if (employees.Count() <= 0)
                return "";

            var employeesString = employees.First().DisplayName;

            return employees.Skip(1).Aggregate(employeesString, (current, employee) => current + (", " + employee.DisplayName));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RouteLogTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object item, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (item as Route == null)
                return "";
            var route = (Route)item;
            var startTime = route.StartTime.ToShortTimeString();
            var endTime = route.EndTime.ToShortTimeString();

            return (startTime + " - " + endTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

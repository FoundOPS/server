using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Sets the Opacity of the RouteDestinations based on whether or not their tasks are of one of the selected RouteTypes
    /// </summary>
    public class RouteDestinationsOpacityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //var currentDestination = values[0] as RouteDestination;
            //var routesVM = values[1] as RoutesVM;
            //var selectedRouteTypes = routesVM.SelectedRouteTypes;

            //// If one of the conditions is met the opacity returned will be 100%
            //// a) There is a null destination passed
            //// b) The destination has no tasks associated with it
            //// c) One of the Tasks has the same type as one of the selected RouteTypes
            //if (currentDestination == null || currentDestination.RouteTasks.Count == 0 || currentDestination.RouteTasks.Any(task => selectedRouteTypes.Contains(task.Name)))
            //    return "1";

            //// Only gets here if the above condtions fail
            //return ".25";
            return 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Sets the Opacity of the RouteTasks based on whether or not they are of one of the selected RouteTypes
    /// </summary>
    public class RouteTasksOpacityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //if (values[0] as RouteTask == null || values[1] as RoutesVM == null)
            //    return ".25";

            //var currentDestination = ((RouteTask)values[0]).RouteDestination;
            //var routesVM = values[1] as RoutesVM;
            //var selectedRouteTypes = routesVM.SelectedRouteTypes;

            //if (currentDestination == null || currentDestination.RouteTasks.Count == 0 || currentDestination.RouteTasks.Any(task => selectedRouteTypes.Contains(task.Name)))
            //    return "1";

            //return ".25";

            return 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

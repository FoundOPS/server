using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Allows the Filter to display the number of Route Tasks that fall under each ServiceType.
    /// Only RouteTasks from Routes with a SelectedRegion are counted.
    /// </summary>
    public class RouteTypeFilterConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var serviceType = values[0] as string;
            var selectedRegions = values[1] as IEnumerable<string>;
            var routesVM = (RoutesVM)values[2];
            var routesDCV = routesVM.DomainCollectionView;
            var selectedRouteTypes = routesVM.SelectedRouteTypes;

            //If the variables have not propogated yet, return nothing
            if (serviceType == null || routesDCV == null || selectedRegions == null)
                return "";

            if (!selectedRouteTypes.Contains(serviceType))
                return String.Format("{0}", serviceType);

            //Filter by ServiceType
            var filteredRoutes = routesDCV.Where(route => route.RouteType == serviceType);

            //Apply the Regions filter only if there are unselected Regions
            if (routesVM.SelectedRegions.Count < routesVM.Regions.Count)
                filteredRoutes =
                    filteredRoutes.Where(filteredRoute =>
                        {
                            //Select the Route's Regions 
                            var routesRegions =
                                filteredRoute.RouteDestinations.Where(rd => rd != null && rd.Location != null && rd.Location.Region != null)
                                    .Select(rd => rd.Location.Region.Name).Distinct().ToArray();

                            //Check if any of the Route's Regions are part of the SelectedRegions
                            return routesRegions.Any(rr => selectedRegions.Any(sr => sr == rr));
                        });

            int count = filteredRoutes.SelectMany(route => route.RouteDestinations).Sum(destination => destination.Tasks.Count(task => task.Name == serviceType));

            //Return "Oil (3)"
            return String.Format("{0} ({1})", serviceType, count);
        }

        /// <summary> 
        /// Converts the specified values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Allows the Filter to display the number of Route Tasks that fall under each Region.
    /// Only RouteTasks from Routes with a SelectedType are counted. 
    /// </summary>
    public class RegionFilterConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var regionName = values[0] as string;
            var selectedTypes = values[1] as IEnumerable<string>;
            var routesVM = (RoutesVM)values[2];
            var selectedRegions = routesVM.SelectedRegions;

            //If the variables have not propogated yet, return nothing
            if (regionName == null || selectedTypes == null) // || routeDCV == null
                return "";

            if (!selectedRegions.Contains(regionName))
                return String.Format("{0}", regionName);

            //Filter Routes by Region name and checks to see if each Route has the region passed to the converter
            var filteredRoutes =
                ((RoutesVM)values[2]).DomainCollectionView.Where(
                    route =>
                    route.RouteDestinations.Where(rd => rd != null && rd.Location != null && rd.Location.Region != null)
                        .Select(rd => rd.Location.Region.Name).Distinct().ToArray().Contains(regionName));

            //Filter by SelectedTypes, only if there are unselected route types

            //Apply the RouteTypes filter only if there are unselected Regions
            if (routesVM.SelectedRouteTypes.Count < routesVM.RouteTypes.Count)
                filteredRoutes = filteredRoutes.Where(filteredRoute => selectedTypes.Contains(filteredRoute.RouteType));

            var countOfDestinations =
                filteredRoutes.Sum(
                    route =>
                    route.RouteDestinations.Count(
                        destination =>
                        destination.Location != null && destination.Location.Region != null &&
                        destination.Location.Region.Name == regionName));

            //Return "Northern Region (3)"
            return String.Format("{0} ({1})", regionName, countOfDestinations);
        }

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

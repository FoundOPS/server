using System;

namespace FoundOps.Common.Tools
{
    /// <summary>
    /// Tools for working with Latitude and Longitudes.
    /// Vincenty formula implementation from https://github.com/lmaslanka/Orthodromic-Distance-Calculator
    /// Intersection logic from http://rbrundritt.wordpress.com/2008/10/20/approximate-points-of-intersection-of-two-line-segments/
    /// </summary>
    public static class GeoLocationTools
    {
        public const double AverageRadiusForSphericalApproximationOfEarth = 6371.01;

        #region Distance

        /// <summary>
        /// Calculate the arc length distance between two locations using the Vincenty formula.
        /// In kilometers.
        /// From https://github.com/lmaslanka/Orthodromic-Distance-Calculator
        /// </summary>
        public static double VincentyDistanceFormula(IGeoLocation locationA, IGeoLocation locationB)
        {
            return AverageRadiusForSphericalApproximationOfEarth * Math.Atan(Math.Sqrt(((Math.Pow(Math.Cos(locationB.LatitudeRad) * Math.Sin(Diff(locationA.LongitudeRad, locationB.LongitudeRad)), 2)) + (Math.Pow((Math.Cos(locationA.LatitudeRad) * Math.Sin(locationB.LatitudeRad)) - (Math.Sin(locationA.LatitudeRad) * Math.Cos(locationB.LatitudeRad) * Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))), 2))) / ((Math.Sin(locationA.LatitudeRad) * Math.Sin(locationB.LatitudeRad)) + (Math.Cos(locationA.LatitudeRad) * Math.Cos(locationB.LatitudeRad) * Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))))));
        }

        /// <summary>
        /// A helper to return the absolute value difference of two doubles
        /// </summary>
        private static double Diff(double a, double b)
        {
            return Math.Abs(b - a);
        }

        #endregion

        #region Intersection

        /// <summary>
        /// Calculate the intersection point of two line segments
        /// adapted to C# from http://rbrundritt.wordpress.com/2008/10/20/approximate-points-of-intersection-of-two-line-segments/
        /// </summary>
        /// <returns>Null if there is no intersection. Otherwise the point of intersection.</returns>
        public static IGeoLocation SimplePolylineIntersection(IGeoLocation latlong1, IGeoLocation latlong2, IGeoLocation latlong3, IGeoLocation latlong4)
        {
            //Line segment 1 (p1, p2)
            var a1 = latlong2.Latitude - latlong1.Latitude;
            var b1 = latlong1.Longitude - latlong2.Longitude;
            var c1 = a1 * latlong1.Longitude + b1 * latlong1.Latitude;

            //Line segment 2 (p3,  p4)
            var a2 = latlong4.Latitude - latlong3.Latitude;
            var b2 = latlong3.Longitude - latlong4.Longitude;
            var c2 = a2 * latlong3.Longitude + b2 * latlong3.Latitude;

            var determinate = a1 * b2 - a2 * b1;

            IGeoLocation intersection;
            if (determinate != 0)
            {
                var x = (b2 * c1 - b1 * c2) / determinate;
                var y = (a1 * c2 - a2 * c1) / determinate;

                var intersect = new GeoLocation(y, x);

                if (InBoundedBox(latlong1, latlong2, intersect) && InBoundedBox(latlong3, latlong4, intersect))
                    intersection = intersect;
                else
                    intersection = null;
            }
            else //lines are parallel
                intersection = null;

            return intersection;
        }

        /// <summary>
        /// Check if the calculated point is within the bounding box
        /// </summary>
        /// <param name="latlong1">The first coordinate which makes up the bounded box</param>
        /// <param name="latlong2">The second coordinate which makes up the bounded box</param>
        /// <param name="latlong3">a point that we are checking to see is inside the box</param>
        /// <returns></returns>
        private static bool InBoundedBox(IGeoLocation latlong1, IGeoLocation latlong2, IGeoLocation latlong3)
        {
            bool betweenLats;
            bool betweenLons;

            if (latlong1.Latitude < latlong2.Latitude)
                betweenLats = (latlong1.Latitude <= latlong3.Latitude &&
                               latlong2.Latitude >= latlong3.Latitude);
            else
                betweenLats = (latlong1.Latitude >= latlong3.Latitude &&
                               latlong2.Latitude <= latlong3.Latitude);

            if (latlong1.Longitude < latlong2.Longitude)
                betweenLons = (latlong1.Longitude <= latlong3.Longitude &&
                               latlong2.Longitude >= latlong3.Longitude);
            else
                betweenLons = (latlong1.Longitude >= latlong3.Longitude &&
                               latlong2.Longitude <= latlong3.Longitude);

            return (betweenLats && betweenLons);
        }

        #endregion
    }

    public interface IGeoLocation
    {
        double Longitude { get; }
        double Latitude { get; }

        double LatitudeRad { get; }
        double LongitudeRad { get; }
    }

    public class GeoLocation : IGeoLocation
    {
        public GeoLocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        #region Properties

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public double LongitudeRad
        {
            get { return DegreeToRadian(Longitude); }
        }

        public double LatitudeRad
        {
            get { return DegreeToRadian(Latitude); }
        }

        #endregion

        #region Helper Functions

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        #endregion
    }
}
using System;

namespace FoundOps.Common.Tools
{
    /// <summary>
    /// Uses VincentyFormula
    /// </summary>
    public static class OrthodromicDistanceCalculator
    {
        public const double AverageRadiusForSphericalApproximationOfEarth = 6371.01;

        public static double OrthodromicDistance(GeoLocation locationA, GeoLocation locationB)
        {
            var distance = (AverageRadiusForSphericalApproximationOfEarth * ArcLengthVincentyFormula(locationA, locationB));

            return distance;
        }

        private static double ArcLengthVincentyFormula(GeoLocation locationA, GeoLocation locationB)
        {
            return
                Math.Atan(
                    Math.Sqrt(
                        ((Math.Pow(
                            Math.Cos(locationB.LatitudeRad)*
                            Math.Sin(Diff(locationA.LongitudeRad, locationB.LongitudeRad)), 2)) +
                         (Math.Pow(
                             (Math.Cos(locationA.LatitudeRad)*Math.Sin(locationB.LatitudeRad)) -
                             (Math.Sin(locationA.LatitudeRad)*Math.Cos(locationB.LatitudeRad)*
                              Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))), 2)))/
                        ((Math.Sin(locationA.LatitudeRad)*Math.Sin(locationB.LatitudeRad)) +
                         (Math.Cos(locationA.LatitudeRad)*Math.Cos(locationB.LatitudeRad)*
                          Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))))));
        }

        public static double Sin2(double x)
        {
            return 0.5 - 0.5 * Math.Cos(2 * x);
        }

        public static double Diff(double a, double b)
        {
            return Math.Abs(b - a);
        }
    }

    public class GeoLocation
    {
        #region Properties

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public double LongitudeRad { get { return DegreeToRadian(Longitude); } }
        public double LatitudeRad { get { return DegreeToRadian(Latitude); } }

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

        public double Proximity(GeoLocation other)
        {
            return OrthodromicDistanceCalculator.OrthodromicDistance(this, other);
        }
    }
}

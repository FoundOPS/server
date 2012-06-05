using System;

namespace FoundOps.Common.Composite.Tools
{
    public class OrthodromicDistanceCalculator
    {
        public const double c_AverageRadiusForSphericalApproximationOfEarth = 6371.01;

        public enum FormulaType
        {
            SphericalLawOfCosinesFormula = 1,
            HaversineFormula = 2,
            VincentyFormula = 3
        };

        public double OrthodromicDistance(GeoLocation locationA, GeoLocation locationB, FormulaType formula)
        {
            var distance = (c_AverageRadiusForSphericalApproximationOfEarth * ArcLength(locationA, locationB, formula));

            //TODO LOG if NAN
            //if(double.IsNaN(distance))
            //{
            //    Log Error
            //}

            return distance;
        }

        private double ArcLength(GeoLocation locationA, GeoLocation locationB, FormulaType formula)
        {
            switch (formula)
            {
                case FormulaType.VincentyFormula:
                    return ArcLengthVincentyFormula(locationA, locationB);

                default:
                    return 0;
            }
        }

        private double ArcLengthVincentyFormula(GeoLocation locationA, GeoLocation locationB)
        {
            return Math.Atan(Math.Sqrt(((Math.Pow(Math.Cos(locationB.LatitudeRad) * Math.Sin(Diff(locationA.LongitudeRad, locationB.LongitudeRad)), 2)) + (Math.Pow((Math.Cos(locationA.LatitudeRad) * Math.Sin(locationB.LatitudeRad)) - (Math.Sin(locationA.LatitudeRad) * Math.Cos(locationB.LatitudeRad) * Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))), 2))) / ((Math.Sin(locationA.LatitudeRad) * Math.Sin(locationB.LatitudeRad)) + (Math.Cos(locationA.LatitudeRad) * Math.Cos(locationB.LatitudeRad) * Math.Cos(Diff(locationA.LongitudeRad, locationB.LongitudeRad))))));
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
    }
}

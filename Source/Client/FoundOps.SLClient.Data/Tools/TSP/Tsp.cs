using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.SLClient.Data.Tools.TSP
{
    public class Tsp
    {
        public static IList<GeoLocation> Shuffle(Random rng, IList<GeoLocation> cities)
        {
            var shuffled = new List<GeoLocation>(cities);
            for (var i = cities.Count() - 1; i >= 1; i--)
            {
                var j = rng.Next(i + 1);
                var temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            return shuffled;
        }

        public static IList<GeoLocation> Swap(Random rng, IList<GeoLocation> cities)
        {
            var count = cities.Count();

            var swapped = new List<GeoLocation>(cities);

            var first = rng.Next(count);
            var city = swapped[first];
            swapped.RemoveAt(first);

            var second = rng.Next(count);
            swapped.Insert(second, city);

            if (rng.NextDouble() < 0.1)
            {
                return Decross(rng, swapped);
            }

            return swapped;
        }

        public static double Length(IList<GeoLocation> cities)
        {
            var length = 0d;
            for (var i = 0; i < cities.Count() - 1; i++)
            {
                length = length + Distance(cities[i], cities[i + 1]);
            }

            length = length + Distance(cities[cities.Count() - 1], cities[0]);
            return length;
        }

        public static double Distance(GeoLocation city1, GeoLocation city2)
        {
            return MapTools.VincentyDistanceFormula(city1, city2);
        }

        public static IList<GeoLocation> Decross(Random rng, IList<GeoLocation> cities)
        {
            var count = cities.Count();

            var swapped = new List<GeoLocation>(cities);

            var first = rng.Next(count - 3);
            var city1 = swapped[first];
            var city2 = swapped[first + 1];

            for (var i = first + 2; i < count - 1; i++)
            {
                var city3 = swapped[i];
                var city4 = swapped[i + 1];
                if (MapTools.SimplePolylineIntersection(city1, city2, city3, city4) != null)
                {
                    swapped.Reverse(first + 1, i - first);
                    break;
                }
            }

            return swapped;
        }
    }
}

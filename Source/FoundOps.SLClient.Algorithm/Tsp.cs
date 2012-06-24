using FoundOps.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.SLClient.Algorithm
{
    /// <summary>
    /// Logic to process the traveling salesmen problem
    /// </summary>
    public class Tsp
    {
        public static IList<IGeoLocation> Shuffle(Random rng, IList<IGeoLocation> cities)
        {
            var shuffled = new List<IGeoLocation>(cities);
            for (var i = cities.Count() - 1; i >= 1; i--)
            {
                var j = rng.Next(i + 1);
                var temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            return shuffled;
        }

        public static IList<IGeoLocation> Swap(Random rng, IList<IGeoLocation> cities)
        {
            var count = cities.Count();

            var swapped = new List<IGeoLocation>(cities);

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

        public static double Length(IList<IGeoLocation> cities)
        {
            var length = 0d;
            for (var i = 0; i < cities.Count() - 1; i++)
            {
                length = length + Distance(cities[i], cities[i + 1]);
            }

            length = length + Distance(cities[cities.Count() - 1], cities[0]);
            return length;
        }

        public static double Distance(IGeoLocation city1, IGeoLocation city2)
        {
            return GeoLocationTools.VincentyDistanceFormula(city1, city2);
        }

        public static IList<IGeoLocation> Decross(Random rng, IList<IGeoLocation> cities)
        {
            var count = cities.Count();

            var swapped = new List<IGeoLocation>(cities);

            var first = rng.Next(count - 3);
            var city1 = swapped[first];
            var city2 = swapped[first + 1];

            for (var i = first + 2; i < count - 1; i++)
            {
                var city3 = swapped[i];
                var city4 = swapped[i + 1];
                if (GeoLocationTools.SimplePolylineIntersection(city1, city2, city3, city4) != null)
                {
                    swapped.Reverse(first + 1, i - first);
                    break;
                }
            }

            return swapped;
        }
    }
}

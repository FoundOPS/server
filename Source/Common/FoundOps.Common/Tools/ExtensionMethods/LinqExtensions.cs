using System;
using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Common.Composite.Tools
{
    public static class LinqExtensions
    {
        /// <summary>
        /// This will return the min element based on the selector function.
        /// Note that this will throw an exception if the sequence is empty, and will return the first element with the minimal value if there's more than one.
        /// </summary>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }


        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            if (source == null)
                throw new ArgumentException();

            try
            {
                var v = source
                    .Select((item, index) => new {item, position = index })
                    .Where((x, index) => match(x.item)).FirstOrDefault();

                if (v == null)
                    return -1;

                return v.position;
            }
            catch
            {
                return -1;
            }
        }

    }
}

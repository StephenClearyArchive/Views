using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    public static class ListExtensions
    {
        public static IList<T> ReverseView<T>(this IList<T> source)
        {
            return new Util.ReverseList<T>(source);
        }

        public static IList<T> View<T>(this IList<T> source, int start = 0, int stop = int.MaxValue, int step = 1)
        {
            if (step <= 0)
                throw new ArgumentOutOfRangeException("step", "Invalid step " + step);

            var count = source.Count;

            // Handle negative start/stop values.
            if (start < 0)
                start += count;
            if (stop < 0)
                stop = stop + count;

            // Clamp start/stop values to [0, count).
            start = Math.Max(start, 0); // start >= 0
            stop = Math.Min(stop, count); // stop <= count

            // Ranges of negative size cause problems when the returned collection is modified (e.g., Add).
            if (stop < start)
            {
                return new Util.AnonymousReadOnlyList<T>
                {
                    Count = () => 0,
                };
            }

            // Apply the slice if necessary.
            source = (start == 0 && stop == count) ? source : new Util.SliceList<T>(source, start, stop - start);

            // Apply the step if necessary.
            return (step == 1) ? source : new Util.StepList<T>(source, step);
        }

        public static IList<TResult> View<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> read = null, Func<TResult, TSource> write = null)
        {
            if (write == null)
            {
                return new Util.AnonymousReadOnlyList<TResult>
                {
                    Count = () => source.Count,
                    GetItem = i => read(source[i]),
                };
            }

            return new Util.ProjectionList<TSource, TResult>(source, read, write);
        }
    }
}

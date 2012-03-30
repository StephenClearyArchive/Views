using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// Provides extension methods for using views over <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Creates a reversed view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <returns>The reversed view.</returns>
        public static IList<T> ReverseView<T>(this IList<T> source)
        {
            return new Util.ReverseList<T>(source);
        }

        /// <summary>
        /// Creates a sliced view of the data. The view includes every <paramref name="step"/> elements in the range [<paramref name="start"/>, <paramref name="stop"/>). Empty slices (where <paramref name="start"/> == <paramref name="stop"/>) are valid views over the source. Negative slices (where <paramref name="start"/> &gt; <paramref name="stop"/>) are empty views not connected to the source.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="start">The index at which to start the slice (inclusive). Defaults to <c>0</c>. If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is less than <c>-source.Count</c>, then it is treated as <c>0</c>.</param>
        /// <param name="stop">The index at which to end the slice (exclusive). Defaults to <c>int.MaxValue</c>. If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is greater than <c>source.Count</c>, then it is treated as <c>source.Count</c>.</param>
        /// <param name="step">The stride of the slice. Defaults to <c>1</c>. This value may not be less than <c>1</c>.</param>
        /// <returns>The sliced view.</returns>
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

        /// <summary>
        /// Creates a projected view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element contained in the source list.</typeparam>
        /// <typeparam name="TResult">The type of virtual element in the projected view.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
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

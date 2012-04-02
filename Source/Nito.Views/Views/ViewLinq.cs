using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// Provides extension methods for using views.
    /// </summary>
    public static class ViewLinq
    {
        /// <summary>
        /// Creates a view of the (typed) data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list and observed by the view.</typeparam>
        /// <param name="source">The source list.</param>
        /// <returns>The view wrapper.</returns>
        public static IView<T> View<T>(this IList<T> source)
        {
            return new Util.SourceListBase<T>(source);
        }

        /// <summary>
        /// Creates a view of the (untyped) data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source list.</param>
        /// <returns>The view wrapper.</returns>
        public static IView<T> View<T>(this System.Collections.IList source)
        {
            return new Util.GenericList<T>(source);
        }

        /// <summary>
        /// Creates a reversed view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>The reversed view.</returns>
        public static IView<T> Reverse<T>(this IView<T> source)
        {
            return new Util.ReverseList<T>(source.AsList());
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
        public static IView<T> Slice<T>(this IView<T> source, int start = 0, int stop = int.MaxValue, int step = 1)
        {
            if (step <= 0)
                throw new ArgumentOutOfRangeException("step", "Invalid step " + step);

            var count = source.AsList().Count;

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
            source = (start == 0 && stop == count) ? source : new Util.SliceList<T>(source.AsList(), start, stop - start);

            // Apply the step if necessary.
            return (step == 1) ? source : new Util.StepList<T>(source.AsList(), step);
        }

        /// <summary>
        /// Creates a sliced view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="offset">The index at which to start the slice (inclusive). If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is less than <c>-source.Count</c>, then it is treated as <c>0</c>.</param>
        /// <returns>The sliced view.</returns>
        public static IView<T> Skip<T>(this IView<T> source, int offset)
        {
            return Slice<T>(source, start: offset);
        }

        /// <summary>
        /// Creates a sliced view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="count">The number of elements in the view. If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is greater than <c>source.Count</c>, then it is treated as <c>source.Count</c>.</param>
        /// <returns>The sliced view.</returns>
        public static IView<T> Take<T>(this IView<T> source, int count)
        {
            return Slice<T>(source, stop: count);
        }

        /// <summary>
        /// Creates a repeating view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="count">The number of times the source view is repeated. This must be greater than or equal to <c>0</c>.</param>
        /// <returns>The repeated view.</returns>
        public static IView<T> Repeat<T>(this IView<T> source, int count)
        {
            var list = source.AsList();
            return Views.View.Generate<T>(i => list[i % list.Count], () => list.Count * count);
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
        public static IView<TResult> Select<TSource, TResult>(this IView<TSource> source, Func<TSource, TResult> read = null, Func<TResult, TSource> write = null)
        {
            return Select<TSource, TResult>(
                source,
                (read == null) ? (Func<TSource, int, TResult>)null : (item, _) => read(item),
                (write == null) ? (Func<TResult, int, TSource>)null : (item, _) => write(item));
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
        public static IView<TResult> Select<TSource, TResult>(this IView<TSource> source, Func<TSource, int, TResult> read = null, Func<TResult, int, TSource> write = null)
        {
            return new Util.ProjectionList<TSource, TResult>(source.AsList(), read, write);
        }

        /// <summary>
        /// Creates a projected, flattened view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TResult">The type of virtual element in the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection.</param>
        /// <returns>The projected, flattened view.</returns>
        public static IView<TResult> SelectMany<TSource, TResult>(this IView<TSource> source, Func<TSource, IView<TResult>> selector)
        {
            return source.Select(selector).Concat();
        }

        /// <summary>
        /// Creates a projected, flattened view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TResult">The type of virtual element in the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection.</param>
        /// <returns>The projected, flattened view.</returns>
        public static IView<TResult> SelectMany<TSource, TResult>(this IView<TSource> source, Func<TSource, int, IView<TResult>> selector)
        {
            return source.Select(selector).Concat();
        }

        /// <summary>
        /// Creates a projected, flattened view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TCollection">The type of intermediate element in the projected view.</typeparam>
        /// <typeparam name="TResult">The type of virtual element in the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="collectionSelector">The projection from the source data to the intermediate elements.</param>
        /// <param name="resultSelector">The projection from the source and intermediate data to the elements in the final view.</param>
        /// <returns>The projected, flattened view.</returns>
        public static IView<TResult> SelectMany<TSource, TCollection, TResult>(this IView<TSource> source, Func<TSource, IView<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return source.Select(item => collectionSelector(item).Select(collection => new { Item = item, Collection = collection })).Concat()
                .Select(x => resultSelector(x.Item, x.Collection));
        }

        /// <summary>
        /// Creates a projected, flattened view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TCollection">The type of intermediate element in the projected view.</typeparam>
        /// <typeparam name="TResult">The type of virtual element in the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="collectionSelector">The projection from the source data to the intermediate elements.</param>
        /// <param name="resultSelector">The projection from the source and intermediate data to the elements in the final view.</param>
        /// <returns>The projected, flattened view.</returns>
        public static IView<TResult> SelectMany<TSource, TCollection, TResult>(this IView<TSource> source, Func<TSource, int, IView<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return source.Select((item, i) => collectionSelector(item, i).Select(collection => new { Item = item, Collection = collection })).Concat()
                .Select(x => resultSelector(x.Item, x.Collection));
        }

        /// <summary>
        /// Creates a sorted view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="comparer">The source comparer. If this is <c>null</c>, then <see cref="Comparer<T>.Default"/> is used.</param>
        /// <returns>The sorted view.</returns>
        public static IView<T> Sort<T>(this IView<T> source, IComparer<T> comparer = null)
        {
            var ret = new Util.IndirectList<T>(source.AsList());
            ((List<int>)ret.Indices).Sort(ret.GetComparer(comparer));
            return ret;
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="others">The additional lists to concatenate to the source list.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IView<T> source, params IView<T>[] others)
        {
            return Enumerable.Repeat(source, 1).Concat(others).Concat();
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source lists to concatenate.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IEnumerable<IView<T>> source)
        {
            return new Util.ConcatList<T>(source.Select(x => x.AsList()));
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source lists to concatenate.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IView<IView<T>> source)
        {
            return source.AsList().Concat();
        }

        /// <summary>
        /// Creates a rotated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="offset">The number of elements to rotate. This may be negative to count from the end of the list.</param>
        /// <returns>The rotated view.</returns>
        public static IView<T> Rotate<T>(this IView<T> source, int offset)
        {
            return source.Slice(start: offset).Concat(source.Slice(stop: offset));
        }
    }
}

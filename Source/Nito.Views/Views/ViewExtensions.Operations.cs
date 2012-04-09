using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    public static partial class ViewExtensions
    {
        /// <summary>
        /// Creates a view of the (typed) data.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the list and observed by the view.</typeparam>
        /// <param name="source">The source list.</param>
        /// <returns>The view wrapper.</returns>
        public static IView<T> View<T>(this IList<T> source)
        {
            var view = source as IView<T>;
            if (view != null)
                return view;
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
            var view = source as IView<T>;
            if (view != null)
                return view;
            return new Util.GenericList<T>(source);
        }

        /// <summary>
        /// Creates a new list containing the elements currently in the specified view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>A new list containing the elements currently in the specified view.</returns>
        public static List<T> ToList<T>(this IView<T> view)
        {
            return new List<T>(view as IList<T>);
        }

        /// <summary>
        /// Creates a new array containing the elements currently in the specified view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>A new array containing the elements currently in the specified view.</returns>
        public static T[] ToArray<T>(this IView<T> view)
        {
            return (view as IList<T>).ToArray();
        }

        /// <summary>
        /// Creates a reversed view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>The reversed view.</returns>
        public static IView<T> Reverse<T>(this IView<T> source)
        {
            return new Util.ReverseList<T>(source as IList<T>);
        }

        /// <summary>
        /// Creates a sliced view of the data. The view includes every <paramref name="step"/> elements in the range [<paramref name="start"/>, <paramref name="stop"/>). Empty slices (where <paramref name="start"/> == <paramref name="stop"/>) are valid views over the source. Negative slices (where <paramref name="start"/> &gt; <paramref name="stop"/>) are read-only, empty views not connected to the source.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="start">The index at which to start the slice (inclusive). Defaults to <c>0</c>. If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is less than <c>-source.Count</c>, then it is treated as <c>0</c>.</param>
        /// <param name="stop">The index at which to end the slice (exclusive). Defaults to <c>int.MaxValue</c>. If this is in the range <c>[-source.Count, -1]</c>, then it is treated as an index from the end of the source. If this is greater than <c>source.Count</c>, then it is treated as <c>source.Count</c>.</param>
        /// <param name="step">The stride of the slice. Defaults to <c>1</c>. This value may not be less than <c>1</c>.</param>
        /// <returns>The sliced view.</returns>
        public static IView<T> Slice<T>(this IView<T> source, int start = 0, int stop = int.MaxValue, int step = 1)
        {
            if (step <= 0)
                throw new ArgumentOutOfRangeException("step", "Invalid step " + step);

            var count = (source as IList<T>).Count;

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
                return Views.View.Empty<T>();

            // Apply the slice if necessary.
            source = (start == 0 && stop == count) ? source : new Util.SliceList<T>(source as IList<T>, start, stop - start);

            // Apply the step if necessary.
            return (step == 1) ? source : new Util.StepList<T>(source as IList<T>, step);
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
        /// <param name="repeatCount">The number of times the source view is repeated. This must be greater than or equal to <c>0</c>.</param>
        /// <returns>The repeated view.</returns>
        public static IView<T> Repeat<T>(this IView<T> source, int repeatCount)
        {
            return new Util.RepeatList<T>(source as IList<T>, repeatCount);
        }

        /// <summary>
        /// Creates a sorted view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="comparer">The source comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        /// <returns>The sorted view.</returns>
        public static IView<T> Sort<T>(this IView<T> source, IComparer<T> comparer = null)
        {
            return new Util.SortedList<T>(source as IList<T>, comparer);
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="others">The additional views to concatenate to the source view.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IView<T> source, params IView<T>[] others)
        {
            return Enumerable.Repeat(source, 1).Concat(others).Concat();
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source views to concatenate.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IEnumerable<IView<T>> source)
        {
            return new Util.ConcatList<T>(source.Select(x => x as IList<T>));
        }

        /// <summary>
        /// Creates a concatenated view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source views to concatenate.</param>
        /// <returns>The concatenated view.</returns>
        public static IView<T> Concat<T>(this IView<IView<T>> source)
        {
            return (source as IList<IView<T>>).Concat();
        }

        /// <summary>
        /// Creates a rotated view of the data. The returned view does not support clearing, inserting, or removing elements.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="offset">The number of elements to rotate. This may be negative to count from the end of the view.</param>
        /// <returns>The rotated view.</returns>
        public static IView<T> Rotate<T>(this IView<T> source, int offset)
        {
            if (offset == 0)
                return source;
            return new Util.OffsetList<T>(source as IList<T>, offset);
        }

        /// <summary>
        /// Creates a projected view of two views.
        /// </summary>
        /// <typeparam name="TSource0">The type of element observed by the first source view.</typeparam>
        /// <typeparam name="TSource1">The type of element observed by the second source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source0">The first source view.</param>
        /// <param name="source1">The second source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Zip<TSource0, TSource1, TResult>(this IView<TSource0> source0, IView<TSource1> source1, Func<TSource0, TSource1, TResult> read = null, Func<TResult, Tuple<TSource0, TSource1>> write = null)
        {
            return Zip<TSource0, TSource1, TResult>(
                source0, source1,
                (read == null) ? (Func<TSource0, TSource1, int, TResult>)null : (item0, item1, _) => read(item0, item1),
                (write == null) ? (Func<TResult, int, Tuple<TSource0, TSource1>>)null : (item, _) => write(item));
        }

        /// <summary>
        /// Creates a projected view of two views.
        /// </summary>
        /// <typeparam name="TSource0">The type of element observed by the first source view.</typeparam>
        /// <typeparam name="TSource1">The type of element observed by the second source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source0">The first source view.</param>
        /// <param name="source1">The second source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Zip<TSource0, TSource1, TResult>(this IView<TSource0> source0, IView<TSource1> source1, Func<TSource0, TSource1, int, TResult> read = null, Func<TResult, int, Tuple<TSource0, TSource1>> write = null)
        {
            return new Util.ProjectionList<TSource0, TSource1, TResult>(source0 as IList<TSource0>, source1 as IList<TSource1>, read, write);
        }

        /// <summary>
        /// Creates a projected view of three views.
        /// </summary>
        /// <typeparam name="TSource0">The type of element observed by the first source view.</typeparam>
        /// <typeparam name="TSource1">The type of element observed by the second source view.</typeparam>
        /// <typeparam name="TSource2">The type of element observed by the third source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source0">The first source view.</param>
        /// <param name="source1">The second source view.</param>
        /// <param name="source2">The third source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Zip<TSource0, TSource1, TSource2, TResult>(this IView<TSource0> source0, IView<TSource1> source1, IView<TSource2> source2, Func<TSource0, TSource1, TSource2, TResult> read = null, Func<TResult, Tuple<TSource0, TSource1, TSource2>> write = null)
        {
            return Zip<TSource0, TSource1, TSource2, TResult>(
                source0, source1, source2,
                (read == null) ? (Func<TSource0, TSource1, TSource2, int, TResult>)null : (item0, item1, item2, _) => read(item0, item1, item2),
                (write == null) ? (Func<TResult, int, Tuple<TSource0, TSource1, TSource2>>)null : (item, _) => write(item));
        }

        /// <summary>
        /// Creates a projected view of three views.
        /// </summary>
        /// <typeparam name="TSource0">The type of element observed by the first source view.</typeparam>
        /// <typeparam name="TSource1">The type of element observed by the second source view.</typeparam>
        /// <typeparam name="TSource2">The type of element observed by the third source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source0">The first source view.</param>
        /// <param name="source1">The second source view.</param>
        /// <param name="source2">The third source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Zip<TSource0, TSource1, TSource2, TResult>(this IView<TSource0> source0, IView<TSource1> source1, IView<TSource2> source2, Func<TSource0, TSource1, TSource2, int, TResult> read = null, Func<TResult, int, Tuple<TSource0, TSource1, TSource2>> write = null)
        {
            return new Util.ProjectionList<TSource0, TSource1, TSource2, TResult>(source0 as IList<TSource0>, source1 as IList<TSource1>, source2 as IList<TSource2>, read, write);
        }

        /// <summary>
        /// Pads a view with a background view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The priority source view.</param>
        /// <param name="backgroundSource">The background source view.</param>
        /// <returns>The padded view.</returns>
        public static IView<T> Pad<T>(this IView<T> source, IView<T> backgroundSource)
        {
            return new Util.LayeredList<T>(backgroundSource as IList<T>, source as IList<T>);
        }

        /// <summary>
        /// Pads a view with a default value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="count">The length of the resulting view.</param>
        /// <param name="value">The value with which to pad the source view until it observes at least <paramref name="count"/> elements.</param>
        /// <returns>The padded view.</returns>
        public static IView<T> Pad<T>(this IView<T> source, int count, T value = default(T))
        {
            return source.Pad(Views.View.Repeat(value, count));
        }

        /// <summary>
        /// Creates a projected view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Map<TSource, TResult>(this IView<TSource> source, Func<TSource, TResult> read = null, Func<TResult, TSource> write = null)
        {
            return Map<TSource, TResult>(
                source,
                (read == null) ? (Func<TSource, int, TResult>)null : (item, _) => read(item),
                (write == null) ? (Func<TResult, int, TSource>)null : (item, _) => write(item));
        }

        /// <summary>
        /// Creates a projected view of the data.
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="read">The projection used when reading elements. This may be <c>null</c> if the projected view is write-only.</param>
        /// <param name="write">The projection used when writing elements. This may be <c>null</c> if the projected view is read-only.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Map<TSource, TResult>(this IView<TSource> source, Func<TSource, int, TResult> read = null, Func<TResult, int, TSource> write = null)
        {
            return new Util.ProjectionList<TSource, TResult>(source as IList<TSource>, read, write);
        }

        /// <summary>
        /// Creates a filtered view of the data.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="filter">The filter method to apply on the data.</param>
        /// <returns>The filtered view.</returns>
        public static IView<T> Filter<T>(this IView<T> source, Func<T, bool> filter)
        {
            return new Util.FilteredList<T>(source as IList<T>, filter);
        }

        // TODO: Randomize, TakeWhile/SkipWhile, Buffer (from Rx), Permutations.
    }
}

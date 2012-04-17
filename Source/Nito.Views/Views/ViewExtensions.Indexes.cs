using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views
{
    /// <summary>
    /// Provides extension methods for all views.
    /// </summary>
    public static partial class ViewExtensions
    {
        /// <summary>
        /// Returns the index of the first element in a view that matches a condition, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.FindIndex(Predicate{T})"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The index of the first element in a view that matches the condition, or -1 if no matching elements are found.</returns>
        public static int FirstIndex<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(predicate != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            var count = view.Count;
            for (int i = 0; i != count; ++i)
            {
                if (predicate(view[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the last element in a view that matches a condition, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.FindLastIndex(Predicate{T})"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The index of the last element in a view that matches a condition, or -1 if no matching elements are found.</returns>
        public static int LastIndex<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(predicate != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            var count = view.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                if (predicate(view[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the only element in a view that matches a condition, or -1 if no matching elements are found. Throws <see cref="InvalidOperationException"/> if there are multiple matching elements.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The index of the only element in a view that matches the condition, or -1 if no matching elements are found.</returns>
        public static int SingleIndex<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(predicate != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            var count = view.Count;
            int ret = -1;
            for (int i = 0; i != count; ++i)
            {
                if (predicate(view[i]))
                {
                    if (ret == -1)
                        ret = i;
                    else
                        throw new InvalidOperationException("More than one element matches.");
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the largest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The index of the largest element in the view.</returns>
        public static int MaxIndex<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            comparer = comparer ?? Comparer<T>.Default;
            var count = view.Count;
            if (count == 0)
                return -1;
            var max = view[0];
            var maxIndex = 0;
            for (int i = 1; i != count; ++i)
            {
                var item = view[i];
                if (comparer.Compare(max, item) < 0)
                {
                    max = item;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Returns the index of the smallest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The index of the smallest element in the view.</returns>
        public static int MinIndex<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            comparer = comparer ?? Comparer<T>.Default;
            return view.MaxIndex(new Util.AnonymousComparer<T> { Compare = (x, y) => comparer.Compare(y, x) });
        }

        /// <summary>
        /// Returns the index of the first matching element in a view, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.IndexOf(T)"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The value to find in the view.</param>
        /// <param name="comparer">The equality comparer to use while searching for the item. Defaults to <c>null</c>. If this parameter is <c>null</c>, this method uses the default equality comparer.</param>
        /// <returns>The index of the first matching element in a view, or -1 if no matching elements are found.</returns>
        public static int FirstIndexOf<T>(this IView<T> view, T item, IEqualityComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            comparer = comparer ?? EqualityComparer<T>.Default;
            return view.FirstIndex(x => comparer.Equals(item, x));
        }

        /// <summary>
        /// Returns the index of the last matching element in a view, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.LastIndexOf(T)"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The value to find in the view.</param>
        /// <param name="comparer">The equality comparer to use while searching for the item. Defaults to <c>null</c>. If this parameter is <c>null</c>, this method uses the default equality comparer.</param>
        /// <returns>The index of the last matching element in a view, or -1 if no matching elements are found.</returns>
        public static int LastIndexOf<T>(this IView<T> view, T item, IEqualityComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Ensures(Contract.Result<int>() == -1 || (Contract.Result<int>() >= 0 && Contract.Result<int>() < view.Count));
            comparer = comparer ?? EqualityComparer<T>.Default;
            return view.LastIndex(x => comparer.Equals(item, x));
        }

        /// <summary>
        /// Searches a sorted view for a given value, returning its index if found. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The item to search for in the view.</param>
        /// <param name="comparer">The object used to compare items. Defaults to <c>null</c>. If this parameter is <c>null</c>, the default comparer is used.</param>
        /// <returns>The index of <paramref name="item"/> if it was in the view; otherwise, the bitwise complement of the next larger element in the view.</returns>
        public static int BinarySearch<T>(this IView<T> view, T item, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            comparer = comparer ?? Comparer<T>.Default;
            return view.BinarySearch(x => comparer.Compare(item, x));
        }

        /// <summary>
        /// Searches a sorted view using a given finder function. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <returns>The index of an item that causes <paramref name="finder"/> to return 0, if any; otherwise, the bitwise complement of the next larger element in the view.</returns>
        public static int BinarySearch<T>(this IView<T> view, Func<T, int> finder)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            int begin = 0, end = view.Count;
            return BinarySearchCore(view, finder, ref begin, ref end);
        }

        /// <summary>
        /// Searches a sorted view for a given value, returning the index of the first matching item if found. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The item to search for in the view.</param>
        /// <param name="comparer">The object used to compare items. Defaults to <c>null</c>. If this parameter is <c>null</c>, the default comparer is used.</param>
        /// <returns>The index of the first occurence of <paramref name="item"/> if it was in the view; otherwise, the bitwise complement of the next larger element in the view.</returns>
        public static int LowerBound<T>(this IView<T> view, T item, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            comparer = comparer ?? Comparer<T>.Default;
            return view.LowerBound(x => comparer.Compare(item, x));
        }

        /// <summary>
        /// Searches a sorted view using a given finder function, returning the index of the first matching item if found. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <returns>The index of the first item that causes <paramref name="finder"/> to return 0, if any; otherwise, the bitwise complement of the next larger element in the view.</returns>
        public static int LowerBound<T>(this IView<T> view, Func<T, int> finder)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            int begin = 0, end = view.Count;
            int mid = BinarySearchCore(view, finder, ref begin, ref end);
            if (mid < 0)
            {
                return mid;
            }

            LowerBoundCore(view, finder, ref begin, mid);
            return begin;
        }

        /// <summary>
        /// Searches a sorted view for a given value, returning the index one past the last matching item if found. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The item to search for in the view.</param>
        /// <param name="comparer">The object used to compare items. Defaults to <c>null</c>. If this parameter is <c>null</c>, the default comparer is used.</param>
        /// <returns>The index one past the last occurence of <paramref name="item"/> if it was in the view; otherwise, the bitwise complement of the next larger element in the view.</returns>
        public static int UpperBound<T>(this IView<T> view, T item, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            comparer = comparer ?? Comparer<T>.Default;
            return view.UpperBound(x => comparer.Compare(item, x));
        }

        /// <summary>
        /// Searches a sorted view using a given finder function, returning the index one past the last matching item if found. If not found, the return value is the bitwise complement of the next element larger than the value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <returns>The index one past the last item that causes <paramref name="finder"/> to return 0, if any; otherwise, the bitwise complement of the index one past the last item that causes <paramref name="finder"/> to return a positive result.</returns>
        public static int UpperBound<T>(this IView<T> view, Func<T, int> finder)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            int begin = 0, end = view.Count;
            int mid = BinarySearchCore(view, finder, ref begin, ref end);
            if (mid < 0)
            {
                return mid;
            }

            UpperBoundCore(view, finder, mid, ref end);
            return end;
        }

        /// <summary>
        /// Searches a sorted view for all instances of a given value. Returns a range as a tuple: [Item1, Item2), which may be an empty range.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The item to search for in the view.</param>
        /// <param name="comparer">The object used to compare items. Defaults to <c>null</c>. If this parameter is <c>null</c>, the default comparer is used.</param>
        public static Tuple<int, int> EqualRange<T>(this IView<T> view, T item, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item1 >= 0 && Contract.Result<Tuple<int, int>>().Item1 <= view.Count);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item2 >= 0 && Contract.Result<Tuple<int, int>>().Item2 <= view.Count);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item1 <= Contract.Result<Tuple<int, int>>().Item2);
            comparer = comparer ?? Comparer<T>.Default;
            return view.EqualRange(x => comparer.Compare(item, x));
        }

        /// <summary>
        /// Searches a sorted view using a given finder function. Returns a range as a tuple: [Item1, Item2), which may be an empty range.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        public static Tuple<int, int> EqualRange<T>(this IView<T> view, Func<T, int> finder)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item1 >= 0 && Contract.Result<Tuple<int, int>>().Item1 <= view.Count);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item2 >= 0 && Contract.Result<Tuple<int, int>>().Item2 <= view.Count);
            Contract.Ensures(Contract.Result<Tuple<int, int>>().Item1 <= Contract.Result<Tuple<int, int>>().Item2);
            var begin = 0;
            var end = view.Count;
            int mid = BinarySearchCore(view, finder, ref begin, ref end);
            if (mid < 0)
            {
                return Tuple.Create(~mid, ~mid);
            }

            LowerBoundCore(view, finder, ref begin, mid);
            UpperBoundCore(view, finder, mid, ref end);
            return Tuple.Create(begin, end);
        }

        /// <summary>
        /// Performs a binary search over a sorted view, returning both a match and the narrowed range.
        /// </summary>
        /// <typeparam name="T">The type of items observed by the view.</typeparam>
        /// <param name="view">The sorted view.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <param name="begin">On input, contains the beginning index at which to search. On output, contains the index of an item less than the found item, or the first item equal to the found item.</param>
        /// <param name="end">On input, contains the ending index at which to search. On output, contains the index one past an item greater than the found item, or the index one past the last item equal to the found item.</param>
        /// <returns>The index of an item that causes <paramref name="finder"/> to return 0, if any; otherwise, the bitwise complement of the next larger element in the list.</returns>
        private static int BinarySearchCore<T>(IView<T> view, Func<T, int> finder, ref int begin, ref int end)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            Contract.Requires(begin >= 0 && begin <= view.Count);
            Contract.Requires(end >= 0 && end <= view.Count);
            Contract.Requires(begin <= end);
            Contract.Ensures(Contract.ValueAtReturn<int>(out begin) >= 0 && Contract.ValueAtReturn<int>(out begin) <= view.Count);
            Contract.Ensures(Contract.ValueAtReturn<int>(out end) >= 0 && Contract.ValueAtReturn<int>(out end) <= view.Count);
            Contract.Ensures(Contract.ValueAtReturn<int>(out begin) <= Contract.ValueAtReturn<int>(out end));
            while (begin != end)
            {
                int mid = begin + ((end - begin) / 2);
                Contract.Assert(mid >= begin);
                Contract.Assert(mid < end);
                int test = finder(view[mid]);
                if (test == 0)
                {
                    return mid;
                }
                else if (test < 0)
                {
                    end = mid;
                }
                else if (test > 0)
                {
                    begin = mid + 1;
                }
            }

            return ~end;
        }

        /// <summary>
        /// Modifies <paramref name="begin"/> so that it refers to the first matching item.
        /// </summary>
        /// <typeparam name="T">The type of items observed by the view.</typeparam>
        /// <param name="view">The sorted view.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <param name="begin">On input, contains the beginning index at which to search. On output, contains the index of the first matching item.</param>
        /// <param name="end">The ending index at which to search. The item at this index must match.</param>
        private static void LowerBoundCore<T>(IView<T> view, Func<T, int> finder, ref int begin, int end)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            Contract.Requires(begin >= 0 && begin <= view.Count);
            Contract.Requires(end >= 0 && end <= view.Count);
            Contract.Requires(begin <= end);
            Contract.Ensures(Contract.ValueAtReturn<int>(out begin) >= 0 && Contract.ValueAtReturn<int>(out begin) <= view.Count);
            Contract.Ensures(Contract.ValueAtReturn<int>(out begin) <= end);
            while (begin != end)
            {
                int mid = begin + ((end - begin) / 2);
                Contract.Assert(mid >= begin);
                Contract.Assert(mid < end);
                int test = finder(view[mid]);
                if (test == 0)
                {
                    end = mid;
                }
                else
                {
                    Contract.Assert(test > 0);
                    begin = mid + 1;
                }
            }
        }

        /// <summary>
        /// Modifies <paramref name="end"/> so that it refers to one past the last matching item.
        /// </summary>
        /// <typeparam name="T">The type of items observed by the view.</typeparam>
        /// <param name="view">The sorted view.</param>
        /// <param name="finder">The finder function to use to find the item. This function should return 0 for a match, a negative value (meaning "search lower") if its parameter is too large, or a positive value (meaning "search higher") if its parameter is too small.</param>
        /// <param name="begin">The beginning index at which to search. The item at this index must match.</param>
        /// <param name="end">On input, contains the ending index at which to search. On output, contains the index one past the last matching item.</param>
        private static void UpperBoundCore<T>(IView<T> view, Func<T, int> finder, int begin, ref int end)
        {
            Contract.Requires(view != null);
            Contract.Requires(finder != null);
            Contract.Requires(begin >= 0 && begin <= view.Count);
            Contract.Requires(end >= 0 && end <= view.Count);
            Contract.Requires(begin <= end);
            Contract.Ensures(Contract.ValueAtReturn<int>(out end) >= 0 && Contract.ValueAtReturn<int>(out end) <= view.Count);
            Contract.Ensures(begin <= Contract.ValueAtReturn<int>(out end));
            while (begin != end)
            {
                int mid = begin + ((end - begin) / 2);
                Contract.Assert(mid >= begin);
                Contract.Assert(mid < end);
                int test = finder(view[mid]);
                if (test == 0)
                {
                    begin = mid + 1;
                }
                else
                {
                    Contract.Assert(test < 0);
                    end = mid;
                }
            }
        }
    }
}

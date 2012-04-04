using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// Provides extension methods for all views.
    /// </summary>
    public static partial class ViewExtensions
    {
        /// <summary>
        /// Returns the index of the first element in a view that matches a condition, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.FindIndex"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The index of the first element in a view that matches the condition, or -1 if no matching elements are found.</returns>
        public static int FirstIndex<T>(this IView<T> view, Func<T, bool> predicate)
        {
            var list = view as IList<T>;
            var count = list.Count;
            for (int i = 0; i != count; ++i)
            {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the last element in a view that matches a condition, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.FindLastIndex"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The index of the last element in a view that matches a condition, or -1 if no matching elements are found.</returns>
        public static int LastIndex<T>(this IView<T> view, Func<T, bool> predicate)
        {
            var list = view as IList<T>;
            var count = list.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
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
            var list = view as IList<T>;
            var count = list.Count;
            int ret = -1;
            for (int i = 0; i != count; ++i)
            {
                if (predicate(list[i]))
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
            comparer = comparer ?? Comparer<T>.Default;
            var list = view as IList<T>;
            var count = list.Count;
            if (count == 0)
                return -1;
            var max = list[0];
            var maxIndex = 0;
            for (int i = 1; i != list.Count; ++i)
            {
                var item = list[i];
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
            comparer = comparer ?? Comparer<T>.Default;
            return view.MaxIndex(new Util.AnonymousComparer<T> { Compare = (x, y) => comparer.Compare(y, x) });
        }

        /// <summary>
        /// Returns the index of the first matching element in a view, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.IndexOf"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The value to find in the view.</param>
        /// <param name="comparer">The equality comparer to use while searching for the item. Defaults to <c>null</c>. If this parameter is <c>null</c>, this method uses the default equality comparer.</param>
        /// <returns>The index of the first matching element in a view, or -1 if no matching elements are found.</returns>
        public static int FirstIndexOf<T>(this IView<T> view, T item, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            return view.FirstIndex(x => comparer.Equals(item, x));
        }

        /// <summary>
        /// Returns the index of the last matching element in a view, or -1 if no matching elements are found. This is logically equivalent to <see cref="List{T}.LastIndexOf"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view in which to locate the value.</param>
        /// <param name="item">The value to find in the view.</param>
        /// <param name="comparer">The equality comparer to use while searching for the item. Defaults to <c>null</c>. If this parameter is <c>null</c>, this method uses the default equality comparer.</param>
        /// <returns>The index of the last matching element in a view, or -1 if no matching elements are found.</returns>
        public static int LastIndexOf<T>(this IView<T> view, T item, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            return view.LastIndex(x => comparer.Equals(item, x));
        }

        // TODO: BinarySearch
    }
}

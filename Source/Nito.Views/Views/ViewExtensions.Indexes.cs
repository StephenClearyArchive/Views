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
            for (int i = 0; i != list.Count; ++i)
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
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
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

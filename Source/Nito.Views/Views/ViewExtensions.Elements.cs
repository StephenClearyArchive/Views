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
        /// Returns the element at a specified index.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="index">The index of the element to return.</param>
        /// <returns>The element at the specified index.</returns>
        public static T ElementAt<T>(this IView<T> view, int index)
        {
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the element at a specified index, or a default value if the index is invalid.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="index">The index of the element to return.</param>
        /// <param name="defaultValue">The default value to return if the index is invalid. Defaults to <c>default(T)</c>.</param>
        /// <returns>The element at the specified index.</returns>
        public static T ElementAtOrDefault<T>(this IView<T> view, int index, T defaultValue = default(T))
        {
            var list = view as IList<T>;
            if (index < 0 || index >= list.Count)
                return defaultValue;
            return list[index];
        }

        public static T First<T>(this IView<T> view)
        {
            var list = view as IList<T>;
            if (list.Count == 0)
                throw new InvalidOperationException("View contains no elements.");
            return list[0];
        }

        public static T First<T>(this IView<T> view, Func<T, bool> predicate)
        {
            var index = view.FirstIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        public static T FirstOrDefault<T>(this IView<T> view, T defaultValue = default(T))
        {
            var list = view as IList<T>;
            if (list.Count == 0)
                return defaultValue;
            return list[0];
        }

        public static T FirstOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            var index = view.FirstIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        public static T Last<T>(this IView<T> view)
        {
            var list = view as IList<T>;
            var count = list.Count;
            if (count == 0)
                throw new InvalidOperationException("View contains no elements.");
            return list[count - 1];
        }

        public static T Last<T>(this IView<T> view, Func<T, bool> predicate)
        {
            var index = view.LastIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        public static T LastOrDefault<T>(this IView<T> view, T defaultValue = default(T))
        {
            var list = view as IList<T>;
            var count = list.Count;
            if (count == 0)
                return defaultValue;
            return list[count - 1];
        }

        public static T LastOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            var index = view.LastIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        public static T Single<T>(this IView<T> view)
        {
            var list = view as IList<T>;
            var count = list.Count;
            if (count != 1)
                throw new InvalidOperationException("View does not contain exactly one element.");
            return list[0];
        }

        public static T Single<T>(this IView<T> view, Func<T, bool> predicate)
        {
            var index = view.SingleIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        public static T SingleOrDefault<T>(this IView<T> view, T defaultValue = default(T))
        {
            var list = view as IList<T>;
            var count = list.Count;
            if (count != 1)
                return defaultValue;
            return list[0];
        }

        public static T SingleOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            var index = view.SingleIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the largest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The largest element in the view.</returns>
        public static T Max<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            var index = view.MaxIndex(comparer);
            if (index == -1)
                return default(T);
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the smallest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The smallest element in the view.</returns>
        public static T Min<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            var index = view.MinIndex(comparer);
            if (index == -1)
                return default(T);
            return (view as IList<T>)[index];
        }
    }
}

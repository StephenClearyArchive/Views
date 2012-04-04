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
        /// Returns the number of elements in a view that satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition to evaluate on each element.</param>
        /// <returns>The number of elements in a view that satisfy a condition.</returns>
        public static int Count<T>(this IView<T> view, Func<T, bool> predicate)
        {
            return (view as IList<T>).Count(predicate);
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

        /// <summary>
        /// Computes the average of the elements in a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double Average(this IView<int> view)
        {
            return (view as IList<int>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view. Returns <c>null</c> if there are no elements or all elements are <c>null</c>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double? Average(this IView<int?> view)
        {
            return (view as IList<int?>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double Average(this IView<long> view)
        {
            return (view as IList<long>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view. Returns <c>null</c> if there are no elements or all elements are <c>null</c>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double? Average(this IView<long?> view)
        {
            return (view as IList<long?>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static float Average(this IView<float> view)
        {
            return (view as IList<float>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view. Returns <c>null</c> if there are no elements or all elements are <c>null</c>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static float? Average(this IView<float?> view)
        {
            return (view as IList<float?>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double Average(this IView<double> view)
        {
            return (view as IList<double>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view. Returns <c>null</c> if there are no elements or all elements are <c>null</c>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static double? Average(this IView<double?> view)
        {
            return (view as IList<double?>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static decimal Average(this IView<decimal> view)
        {
            return (view as IList<decimal>).Average();
        }

        /// <summary>
        /// Computes the average of the elements in a view. Returns <c>null</c> if there are no elements or all elements are <c>null</c>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The average of the elements in a view.</returns>
        public static decimal? Average(this IView<decimal?> view)
        {
            return (view as IList<decimal?>).Average();
        }
    }
}

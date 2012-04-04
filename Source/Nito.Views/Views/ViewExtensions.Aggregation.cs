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
    }
}

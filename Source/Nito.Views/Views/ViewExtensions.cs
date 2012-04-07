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
        /// Returns the number of elements in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <returns>The number of elements in the view.</returns>
        public static int Count<T>(this IView<T> view)
        {
            return (view as IList<T>).Count;
        }

        /// <summary>
        /// Copies all elements from one view into another view. The elements are copied in index order.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The source view.</param>
        /// <param name="destination">The destination view.</param>
        public static void CopyTo<T>(this IView<T> view, IView<T> destination)
        {
            var list = view as IList<T>;
            var destinationList = destination as IList<T>;
            var count = list.Count;
            for (int i = 0; i != count; ++i)
                destinationList[i] = list[i];
        }

        /// <summary>
        /// Compares two sequences and determines if they are equal, using the specified element equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the views.</typeparam>
        /// <param name="view">The first source view.</param>
        /// <param name="other">The second source view.</param>
        /// <param name="comparer">The comparison object used to compare elements for equality. If this parameter is <c>null</c>, this method uses the default element equality comparer.</param>
        /// <returns><c>true</c> if every element in both views are equal; otherwise, <c>false</c>.</returns>
        public static bool SequenceEqual<T>(this IView<T> view, IView<T> other, IEqualityComparer<T> comparer = null)
        {
            var list = view as IList<T>;
            var otherList = other as IList<T>;
            if (list.Count != otherList.Count)
                return false;
            comparer = comparer ?? EqualityComparer<T>.Default;

            return list.SequenceEqual(otherList, comparer);
        }
    }
}

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
        /// Compares two sequences and determines if they are equal, using the specified element equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the views.</typeparam>
        /// <param name="view">The first source view.</param>
        /// <param name="other">The second source view.</param>
        /// <param name="comparer">The comparison object used to compare elements for equality. If this parameter is <c>null</c>, this method uses the default element equality comparer.</param>
        /// <returns><c>true</c> if every element in both views are equal; otherwise, <c>false</c>.</returns>
        public static bool SequenceEqual<T>(this IView<T> view, IView<T> other, IEqualityComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Requires(other != null);
            if (view.Count != other.Count)
                return false;
            comparer = comparer ?? EqualityComparer<T>.Default;
            var count = view.Count;

            for (int i = 0; i != count; ++i)
            {
                if (!comparer.Equals(view[i], other[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two sequences lexicographically, using the specified element comparer. Returns a negative value if this view is less than <paramref name="other"/>, 0 if they are equivalent, and a positive value if this view is greater than <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the views.</typeparam>
        /// <param name="view">The first source view.</param>
        /// <param name="other">The second source view.</param>
        /// <param name="comparer">The comparison object used to compare elements. If this parameter is <c>null</c>, this method uses the default element comparer.</param>
        /// <returns>A negative value if this view is less than <paramref name="other"/>, 0 if they are equivalent, and a positive value if this view is greater than <paramref name="other"/>.</returns>
        public static int SequenceCompare<T>(this IView<T> view, IView<T> other, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Requires(other != null);
            comparer = comparer ?? Comparer<T>.Default;
            var count = Math.Min(view.Count, other.Count);

            for (int i = 0; i != count; ++i)
            {
                var ret = comparer.Compare(view[i], other[i]);
                if (ret != 0)
                    return ret;
            }

            if (view.Count < other.Count)
                return -1;
            else if (view.Count == other.Count)
                return 0;
            return 1;
        }
    }
}

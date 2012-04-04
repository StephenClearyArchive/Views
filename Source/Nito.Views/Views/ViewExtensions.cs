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
        /// Adds an item to the end of this view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="item">The object to add to this view.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// This view is read-only.
        /// </exception>
        public static void Add<T>(this IView<T> view, T item)
        {
            (view as IList<T>).Add(item);
        }

        /// <summary>
        /// Determines whether this view contains a specific value.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="item">The object to locate in this view.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this view; otherwise, false.
        /// </returns>
        public static bool Contains<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).Contains(item);
        }

        /// <summary>
        /// Copies the elements of this view to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from this view. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source view is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public static void CopyTo<T>(this IView<T> view, T[] array, int arrayIndex)
        {
            (view as IList<T>).CopyTo(array, arrayIndex);
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
        /// Removes the first occurrence of a specific object from this view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="item">The object to remove from this view.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from this view; otherwise, false. This method returns false if <paramref name="item"/> is not found in this view.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// This view is read-only.
        /// </exception>
        public static bool Remove<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).Remove(item);
        }

        /// <summary>
        /// Returns the last element of a view that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="list">The view in which to locate the value.</param>
        /// <param name="match">The delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element of the view that returned true from <paramref name="match"/>.</returns>
        /// <exception cref="InvalidOperationException">No element satisfies the condition.</exception>
        public static T Last<T>(this IView<T> list, Func<T, bool> match)
        {
            return (list.Reverse() as IList<T>).First(match);
        }

        /// <summary>
        /// Returns the last element of a view that satisfies a specified condition, or a default value if no element is found.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="list">The view in which to locate the value.</param>
        /// <param name="match">The delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element of the view that returned true from <paramref name="match"/>, or <c>default(T)</c> if no element is found.</returns>
        public static T LastOrDefault<T>(this IView<T> list, Func<T, bool> match)
        {
            return (list.Reverse() as IList<T>).FirstOrDefault(match);
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

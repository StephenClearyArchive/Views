using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// Provides extension methods for all views.
    /// </summary>
    public static class ViewExtensions
    {
        /// <summary>
        /// Adds an item to the end of this list.
        /// </summary>
        /// <param name="item">The object to add to this list.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public static void Add<T>(this IView<T> view, T item)
        {
            (view as IList<T>).Add(item);
        }

        /// <summary>
        /// Determines whether this list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this list; otherwise, false.
        /// </returns>
        public static bool Contains<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).Contains(item);
        }

        /// <summary>
        /// Copies the elements of this list to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from this slice. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
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
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public static void CopyTo<T>(this IView<T> view, T[] array, int arrayIndex)
        {
            (view as IList<T>).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines the index of a specific item in this list.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>The index of <paramref name="item"/> if found in this list; otherwise, -1.</returns>
        public static int IndexOf<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).IndexOf(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from this list.
        /// </summary>
        /// <param name="item">The object to remove from this list.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from this list; otherwise, false. This method also returns false if <paramref name="item"/> is not found in this list.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public static bool Remove<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).Remove(item);
        }
    }
}

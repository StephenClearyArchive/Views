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
        /// Adds an item to the end of this view.
        /// </summary>
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
        /// Determines the index of a specific item in this view.
        /// </summary>
        /// <param name="item">The object to locate in this view.</param>
        /// <returns>The index of <paramref name="item"/> if found in this view; otherwise, -1.</returns>
        public static int IndexOf<T>(this IView<T> view, T item)
        {
            return (view as IList<T>).IndexOf(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from this view.
        /// </summary>
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
    }
}

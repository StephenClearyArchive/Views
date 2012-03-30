using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Provides a delegate-based implementation of a read-only list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    internal sealed class AnonymousReadOnlyList<T> : ReadOnlyListBase<T>
    {
        /// <summary>
        /// The delegate used to get the number of items in the list.
        /// </summary>
        public new Func<int> Count { get; set; }

        /// <summary>
        /// The delegate used to get items in the list.
        /// </summary>
        public Func<int, T> GetItem { get; set; }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.Count();
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.GetItem(index);
        }
    }
}

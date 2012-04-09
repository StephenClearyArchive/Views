using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Provides implementations of most list methods, for any list types that either do not allow item modification OR do not allow list modification. The default implementations of the methods in this class do not allow either.
    /// </summary>
    /// <typeparam name="T">The type of item contained in the list.</typeparam>
    public abstract class ReadOnlySourceListBase<T> : SourceListBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySourceListBase{T}"/> class over the specified list.
        /// </summary>
        /// <param name="source">The source list.</param>
        public ReadOnlySourceListBase(IList<T> source)
            : base(source)
        {
            Contract.Requires(source != null);
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="ICollection{T}.Add"/>, <see cref="ICollection{T}.Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected override bool CanUpdateCollection()
        {
            return false;
        }

        /// <summary>
        /// Removes all elements from the list. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        protected override void DoClear()
        {
            throw this.NotSupported();
        }

        /// <summary>
        /// Sets an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            throw this.NotSupported();
        }

        /// <summary>
        /// Inserts an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            throw this.NotSupported();
        }

        /// <summary>
        /// Removes an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            throw this.NotSupported();
        }
    }
}

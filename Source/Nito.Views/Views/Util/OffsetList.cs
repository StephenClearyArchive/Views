using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Applies an index offset into a source list, with modulus. This list does not support clearing, inserting, or removing elements.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class OffsetList<T> : SourceListBase<T>
    {
        /// <summary>
        /// The offset into the source list where this list begins. This offset is added when reading and subtracted when writing.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="offset">The offset into the source list where this list begins. This offset may be negative.</param>
        public OffsetList(IList<T> source, int offset)
            : base(source)
        {
            this.offset = offset;
        }

        /// <summary>
        /// A notification that the source collection has added an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has removed an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has replaced an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Replaced(this.SourceToViewIndex(index), oldItem, newItem);
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="Add"/>, <see cref="Clear"/>, etc.
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
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[this.ViewToSourceIndex(index)];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            using (this.listener.Pause())
            {
                this.source[this.ViewToSourceIndex(index)] = item;
            }
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

        /// <summary>
        /// Takes an index and applies a modulus so that it is in the range <c>[0, <paramref name="maxValueExclusive"/>)</c>.
        /// </summary>
        /// <param name="index">The index to normalize.</param>
        /// <returns>The normalized index.</returns>
        private int NormalizeIndex(int index)
        {
            var count = this.Count;
            if (count == 0)
                return 0;
            index %= count;
            if (index < 0)
                index += count;
            return index;
        }

        /// <summary>
        /// Converts a view index to a source index.
        /// </summary>
        /// <param name="index">The index to convert.</param>
        /// <returns>The converted index.</returns>
        private int ViewToSourceIndex(int index)
        {
            return this.NormalizeIndex(index + this.offset);
        }

        /// <summary>
        /// Converts a source index to a view index.
        /// </summary>
        /// <param name="index">The index to convert.</param>
        /// <returns>The converted index.</returns>
        private int SourceToViewIndex(int index)
        {
            return this.NormalizeIndex(index - this.offset);
        }
    }
}

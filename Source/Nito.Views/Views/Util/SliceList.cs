using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Slices a source list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class SliceList<T> : SourceListBase<T>
    {
        /// <summary>
        /// The offset into the source list where this slice begins.
        /// </summary>
        private int offset;

        /// <summary>
        /// The number of objects in this slice.
        /// </summary>
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="SliceList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="offset">The offset into the source list where this slice begins.</param>
        /// <param name="count">The number of objects in this slice.</param>
        public SliceList(IList<T> source, int offset, int count)
            : base(source)
        {
            Contract.Requires(source != null);
            Contract.Requires(offset >= 0 && offset <= source.Count);
            Contract.Requires(count <= source.Count);
            Contract.Requires(offset <= source.Count - count);

            this.offset = offset;
            this.count = count;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.offset >= 0 && this.offset <= this.source.Count);
            Contract.Invariant(this.count <= this.source.Count);
            Contract.Invariant(this.offset <= this.source.Count - this.count);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            if (index >= this.offset)
            {
                if (index - this.offset <= this.count)
                {
                    ++this.count;
                    this.CreateNotifier().Added(index + this.offset, item);
                }
            }
            else
            {
                ++this.offset;
            }
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
        {
            if (index >= this.offset)
            {
                if (index - this.offset < this.count)
                {
                    --this.count;
                    this.CreateNotifier().Removed(index + this.offset, item);
                }
            }
            else
            {
                --this.offset;
            }
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            if (index >= this.offset && index - this.offset < this.count)
                this.CreateNotifier().Replaced(index + this.offset, oldItem, newItem);
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
        {
            while (this.count > 0)
            {
                this.source.RemoveAt(this.offset);
                --this.count;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.count;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[this.offset + index];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            this.source[this.offset + index] = item;
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            this.source.Insert(this.offset + index, item);
            ++this.count;
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            this.source.RemoveAt(this.offset + index);
            --this.count;
        }
    }
}

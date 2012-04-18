using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Slices a source view.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public class SliceView<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The offset into the source view where this slice begins.
        /// </summary>
        protected int offset;

        /// <summary>
        /// The number of objects in this slice.
        /// </summary>
        protected int sliceCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SliceView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="offset">The offset into the source view where this slice begins.</param>
        /// <param name="count">The number of objects in this slice.</param>
        public SliceView(IView<T> source, int offset, int count)
            : base(source)
        {
            Contract.Requires(source != null);
            Contract.Requires(offset >= 0 && offset <= source.Count);
            Contract.Requires(count >= 0 && count <= source.Count);
            Contract.Requires(offset <= source.Count - count);

            this.offset = offset;
            this.sliceCount = count;
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.sliceCount; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[this.offset + index]; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.offset >= 0 && this.offset <= this.source.Count);
            Contract.Invariant(this.sliceCount >= 0 && this.sliceCount <= this.source.Count);
            Contract.Invariant(this.offset <= this.source.Count - this.sliceCount);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            if (index >= this.offset)
            {
                if (index - this.offset <= this.sliceCount)
                {
                    ++this.sliceCount;
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
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            if (index >= this.offset)
            {
                if (index - this.offset < this.sliceCount)
                {
                    --this.sliceCount;
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
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public override void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            if (index >= this.offset && index - this.offset < this.sliceCount)
                this.CreateNotifier().Replaced(index + this.offset, oldItem, newItem);
        }
    }
}

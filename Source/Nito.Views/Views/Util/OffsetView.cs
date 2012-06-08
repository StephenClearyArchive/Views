using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Applies an index offset into a source view, with modulus.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class OffsetView<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The offset into the source view where this view begins. This offset is added when reading.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="offset">The offset into the source view where this view begins. This offset may be negative.</param>
        public OffsetView(IView<T> source, int offset)
            : base(source)
        {
            this.offset = offset;
        }

        /// <summary>
        /// Gets the offset into the source view where this view begins. This offset may be negative.
        /// </summary>
        public int Offset
        {
            get { return this.offset; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[this.ViewToSourceIndex(index)]; }
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
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
            this.CreateNotifier().Replaced(this.SourceToViewIndex(index), oldItem, newItem);
        }

        /// <summary>
        /// Takes an index and applies a modulus so that it is in the range <c>[0, <see cref="SourceViewBase{T}.Count"/>)</c>.
        /// </summary>
        /// <param name="index">The index to normalize.</param>
        /// <returns>The normalized index.</returns>
        private int NormalizeIndex(int index)
        {
            var count = this.Count;
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

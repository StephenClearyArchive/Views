using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// A view repeated a number of times.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class RepeatView<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The number of times the source view is repeated.
        /// </summary>
        private readonly int repeatCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="repeatCount">The number of times the source view is repeated. This must be greater than <c>0</c>.</param>
        public RepeatView(IView<T> source, int repeatCount)
            : base(source)
        {
            Contract.Requires(source != null);
            Contract.Requires(repeatCount > 0);
            this.repeatCount = repeatCount;
        }

        /// <summary>
        /// Gets the number of times the source view is repeated.
        /// </summary>
        public int RepeatCount
        {
            get { return this.repeatCount; }
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.source.Count * repeatCount; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[index % this.source.Count]; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(repeatCount > 0);
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
            this.CreateNotifier().Reset();
        }
    }
}

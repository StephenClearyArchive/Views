using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A view that is based on a source list.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class ListView<T> : MutableViewBase<T>, ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The source list.
        /// </summary>
        private readonly IList<T> source;

        /// <summary>
        /// The listener for the source view.
        /// </summary>
        private readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListView&lt;T&gt;"/> class over the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        public ListView(IList<T> source)
        {
            Contract.Requires(source != null);
            this.source = source;
            this.listener = CollectionChangedListener<T>.Create(source, this);
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.source.Count; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[index]; }
        }

        public override bool CanNotifyCollectionChanged
        {
            get { return (this.listener != null); }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source != null);
        }

        public void Added(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Added(index, item);
        }

        public void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Removed(index, item);
        }

        public void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        public void Reset(INotifyCollectionChanged collection)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that there is at least one <see cref="ListBase{T}.CollectionChanged"/> or <see cref="ListBase{T}.PropertyChanged"/> subscription active. This implementation activates the source listener.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="ListBase{T}.CollectionChanged"/> nor <see cref="ListBase{T}.PropertyChanged"/> subscriptions active. This implementation deactivates the source listener.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener.Deactivate();
        }
    }
}

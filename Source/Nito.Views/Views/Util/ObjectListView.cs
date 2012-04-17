﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Wraps a non-generic source list, treating it as a generic view.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public sealed class ObjectListView<T> : MutableViewBase<T>, ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The wrapped non-generic list.
        /// </summary>
        private readonly System.Collections.IList source;

        /// <summary>
        /// The listener for the source list.
        /// </summary>
        private readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectListView&lt;T&gt;"/> class with the specified source.
        /// </summary>
        /// <param name="source">The non-generic source list to wrap.</param>
        public ObjectListView(System.Collections.IList source)
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
            get { return (T)this.source[index]; }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Layers a "priority" source view over a "background" source view.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class LayeredView<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The priority source view.
        /// </summary>
        private readonly IView<T> prioritySource;

        /// <summary>
        /// The listener for the priority source view.
        /// </summary>
        private readonly CollectionChangedListener<T> priorityListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredView&lt;T&gt;"/> class over the specified source views.
        /// </summary>
        /// <param name="source">The background source view.</param>
        /// <param name="prioritySource">The priority source view.</param>
        public LayeredView(IView<T> source, IView<T> prioritySource)
            : base(source)
        {
            Contract.Requires(source != null);
            Contract.Requires(prioritySource != null);
            this.prioritySource = prioritySource;
            this.priorityListener = CollectionChangedListener<T>.Create(prioritySource, this);
        }

        /// <summary>
        /// Gets the priority source view.
        /// </summary>
        public IView<T> PrioritySource
        {
            get { return this.prioritySource; }
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get
            {
                return Math.Max(this.source.Count, this.prioritySource.Count);
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get
            {
                if (index < this.prioritySource.Count)
                    return this.prioritySource[index];
                return this.source[index];
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.prioritySource != null);
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
            if (collection == this.prioritySource || index >= this.prioritySource.Count)
                this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="MutableViewBase{T}.CollectionChanged"/> subscription active.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.priorityListener.Activate();
            base.SubscriptionsActive();
        }

        /// <summary>
        /// A notification that there are no <see cref="MutableViewBase{T}.CollectionChanged"/> subscriptions active.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.priorityListener.Deactivate();
            base.SubscriptionsInactive();
        }
    }
}

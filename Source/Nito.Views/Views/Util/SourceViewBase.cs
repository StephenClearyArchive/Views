using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A view that is based on a source view.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public abstract class SourceViewBase<T> : MutableViewBase<T>, ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        protected readonly IView<T> source;

        /// <summary>
        /// The listener for the source view.
        /// </summary>
        protected readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceViewBase&lt;T&gt;"/> class over the specified source view.
        /// </summary>
        /// <param name="source">The source view.</param>
        public SourceViewBase(IView<T> source)
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

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public override bool CanNotifyCollectionChanged
        {
            get { return (this.source as ICanNotifyCollectionChanged).CanNotifyCollectionChanged; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source != null);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public virtual void Added(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Added(index, item);
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public virtual void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Removed(index, item);
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public virtual void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that the source collection has changed significantly.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public virtual void Reset(INotifyCollectionChanged collection)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that there is at least one <see cref="MutableViewBase{T}.CollectionChanged"/> subscription active.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="MutableViewBase{T}.CollectionChanged"/> subscriptions active.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener.Deactivate();
        }
    }
}

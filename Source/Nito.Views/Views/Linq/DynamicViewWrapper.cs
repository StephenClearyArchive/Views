using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;
using Views.Util;

namespace Views.Linq
{
    /// <summary>
    /// Projects a source view of an unknown type to a typed result view using a cast to the result type.
    /// </summary>
    /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
    public sealed class DynamicViewWrapper<TResult> : MutableViewBase<TResult>, ICollectionChangedResponder<object>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        private readonly dynamic source;

        /// <summary>
        /// The listener for the source view.
        /// </summary>
        private readonly CollectionChangedListener<object> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicViewWrapper{TResult}"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        public DynamicViewWrapper(dynamic source)
        {
            this.source = source;
            this.listener = CollectionChangedListener<object>.Create(source, this);
        }

        /// <summary>
        /// Gets the source view.
        /// </summary>
        public dynamic Source
        {
            get { return this.source; }
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
        public override TResult this[int index]
        {
            get { return (TResult)this.source[index]; }
        }

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public override bool CanNotifyCollectionChanged
        {
            get { return (this.source as ICanNotifyCollectionChanged).CanNotifyCollectionChanged; }
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public void Added(INotifyCollectionChanged collection, int index, object item)
        {
            this.CreateNotifier().Added(index, (TResult)item);
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public void Removed(INotifyCollectionChanged collection, int index, object item)
        {
            this.CreateNotifier().Removed(index, (TResult)item);
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public void Replaced(INotifyCollectionChanged collection, int index, object oldItem, object newItem)
        {
            this.CreateNotifier().Replaced(index, (TResult)oldItem, (TResult)newItem);
        }

        /// <summary>
        /// A notification that the source collection has changed significantly. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public void Reset(INotifyCollectionChanged collection)
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

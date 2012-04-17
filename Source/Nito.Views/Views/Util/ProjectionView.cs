using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Projects a source view to a result view.
    /// </summary>
    /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
    /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
    public sealed class ProjectionView<TSource, TResult> : MutableViewBase<TResult>, ICollectionChangedResponder<TSource>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        private readonly IView<TSource> source;

        /// <summary>
        /// The listener for the source view.
        /// </summary>
        private readonly CollectionChangedListener<TSource> listener;

        /// <summary>
        /// The projection function from source to result.
        /// </summary>
        private readonly Func<TSource, TResult> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionView{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection function from source to result.</param>
        public ProjectionView(IView<TSource> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            this.source = source;
            this.selector = selector;
            this.listener = CollectionChangedListener<TSource>.Create(source, this);
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
            get { return this.selector(this.source[index]); }
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
            Contract.Invariant(this.selector != null);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public void Added(INotifyCollectionChanged collection, int index, TSource item)
        {
            this.CreateNotifier().Added(index, this.selector(item));
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public void Removed(INotifyCollectionChanged collection, int index, TSource item)
        {
            this.CreateNotifier().Removed(index, this.selector(item));
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public void Replaced(INotifyCollectionChanged collection, int index, TSource oldItem, TSource newItem)
        {
            this.CreateNotifier().Replaced(index, this.selector(oldItem), this.selector(newItem));
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

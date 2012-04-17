using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Concatenates a sequence of source views into a single view.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class ConcatView<T> : MutableViewBase<T>, ICollectionChangedResponder<IView<T>>, ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The sequence of source views.
        /// </summary>
        private readonly IEnumerable<IView<T>> sources;

        /// <summary>
        /// The listener for the sequence of source views.
        /// </summary>
        private readonly CollectionChangedListener<IView<T>> listener;

        /// <summary>
        /// Listeners for each source view.
        /// </summary>
        private IEnumerable<CollectionChangedListener<T>> listeners;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcatView&lt;T&gt;"/> class with the specified source views.
        /// </summary>
        /// <param name="sources">The source views to concatenate.</param>
        public ConcatView(IEnumerable<IView<T>> sources)
        {
            Contract.Requires(sources != null);
            Contract.Requires(Contract.ForAll(sources, x => x != null));
            this.sources = sources;
            this.listener = CollectionChangedListener<IView<T>>.Create(sources, this);
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, this));
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.sources.Sum(x => x.Count); }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get
            {
                IView<T> source;
                int sourceIndex;
                this.FindExistingIndex(index, out source, out sourceIndex);
                return source[sourceIndex];
            }
        }

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public override bool CanNotifyCollectionChanged
        {
            get
            {
                var sourcesCanNotify = sources as ICanNotifyCollectionChanged;
                if (sourcesCanNotify != null && sourcesCanNotify.CanNotifyCollectionChanged)
                    return true;
                return this.sources.Any(x => (x as ICanNotifyCollectionChanged).CanNotifyCollectionChanged);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.sources != null);
            Contract.Invariant(this.listeners != null);
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        public void Added(INotifyCollectionChanged collection, int index, IView<T> item)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, this));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        public void Removed(INotifyCollectionChanged collection, int index, IView<T> item)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, this));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        public void Replaced(INotifyCollectionChanged collection, int index, IView<T> oldItem, IView<T> newItem)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, this));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed or that one of the source collections has changed.
        /// </summary>
        public void Reset(INotifyCollectionChanged collection)
        {
            if (collection == sources)
                this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, this));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has added an item.
        /// </summary>
        public void Added(INotifyCollectionChanged collection, int index, T item)
        {
            var sourceBaseIndex = this.FindBaseIndex(collection as IView<T>);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Added(sourceBaseIndex + index, item);
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has removed an item.
        /// </summary>
        public void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            var sourceBaseIndex = this.FindBaseIndex(collection as IView<T>);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Removed(sourceBaseIndex + index, item);
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has replaced an item.
        /// </summary>
        public void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            var sourceBaseIndex = this.FindBaseIndex(collection as IView<T>);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Replaced(sourceBaseIndex + index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="MutableViewBase{T}.CollectionChanged"/> subscription active.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener.Activate();
            foreach (var sourceListener in this.listeners)
                sourceListener.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="MutableViewBase{T}.CollectionChanged"/> subscriptions active.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener.Deactivate();
            foreach (var sourceListener in this.listeners)
                sourceListener.Deactivate();
        }

        /// <summary>
        /// Finds the source view and its index for accessing a specified concatenated index.
        /// </summary>
        /// <param name="concatIndex">The concatenated index.</param>
        /// <param name="source">On return, holds the source view corresponding to the concatenated index.</param>
        /// <param name="sourceIndex">On return, holds the source view index corresponding to the concatenated index.</param>
        private void FindExistingIndex(int concatIndex, out IView<T> source, out int sourceIndex)
        {
            Contract.Requires(concatIndex >= 0 && concatIndex < this.Count);
            Contract.Ensures(Contract.ValueAtReturn<IView<T>>(out source) != null);
            Contract.Ensures(Contract.ValueAtReturn<int>(out sourceIndex) >= 0 && Contract.ValueAtReturn<int>(out sourceIndex) < Contract.ValueAtReturn<IView<T>>(out source).Count);
            source = null;
            sourceIndex = concatIndex;
            foreach (var sourceList in this.sources)
            {
                if (sourceIndex < sourceList.Count)
                {
                    source = sourceList;
                    return;
                }

                sourceIndex -= sourceList.Count;
            }
        }

        /// <summary>
        /// Finds the base index for a specified source view.
        /// </summary>
        /// <param name="source">The source view to find.</param>
        /// <returns>The base index for the specified source view.</returns>
        private int FindBaseIndex(IView<T> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() < this.Count);
            int ret = 0;
            foreach (var sourceList in this.sources)
            {
                if (sourceList == source)
                    return ret;
                ret += sourceList.Count;
            }

            return -1;
        }
    }
}

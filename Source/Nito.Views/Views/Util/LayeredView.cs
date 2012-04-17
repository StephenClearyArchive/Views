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

        public override int Count
        {
            get
            {
                return Math.Max(this.source.Count, this.prioritySource.Count);
            }
        }

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

        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        public override void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            if (collection == this.prioritySource || index >= this.prioritySource.Count)
                this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="ListBase{T}.CollectionChanged"/> or <see cref="ListBase{T}.PropertyChanged"/> subscription active. This implementation activates the source listeners.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.priorityListener.Activate();
            base.SubscriptionsActive();
        }

        /// <summary>
        /// A notification that there are no <see cref="ListBase{T}.CollectionChanged"/> nor <see cref="ListBase{T}.PropertyChanged"/> subscriptions active. This implementation deactivates the source listeners.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.priorityListener.Deactivate();
            base.SubscriptionsInactive();
        }
    }
}

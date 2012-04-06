using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Layers a "priority" source list over a "background" source list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class LayeredList<T> : ReadOnlySourceListBase<T>, CollectionChangedListener<T>.IResponder
    {
        /// <summary>
        /// The priority source list.
        /// </summary>
        protected readonly IList<T> prioritySource;

        /// <summary>
        /// The listener for the priority source list.
        /// </summary>
        protected readonly CollectionChangedListener<T> priorityListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredList&lt;T&gt;"/> class over the specified source lists.
        /// </summary>
        /// <param name="source">The background source list.</param>
        /// <param name="prioritySource">The priority source list.</param>
        public LayeredList(IList<T> source, IList<T> prioritySource)
            : base(source)
        {
            this.prioritySource = prioritySource;
            this.priorityListener = CollectionChangedListener<T>.Create(prioritySource, this);
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only if any of its source lists are read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.source.IsReadOnly || this.prioritySource.IsReadOnly; }
        }

        void CollectionChangedListener<T>.IResponder.Added(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        void CollectionChangedListener<T>.IResponder.Removed(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        void CollectionChangedListener<T>.IResponder.Replaced(int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        void CollectionChangedListener<T>.IResponder.Reset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has added an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has removed an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has replaced an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            if (index >= this.prioritySource.Count)
                this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="CollectionChanged"/> or <see cref="PropertyChanged"/> subscription active. This implementation activates the source listeners.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.priorityListener.Activate();
            base.SubscriptionsActive();
        }

        /// <summary>
        /// A notification that there are no <see cref="CollectionChanged"/> nor <see cref="PropertyChanged"/> subscriptions active. This implementation deactivates the source listeners.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.priorityListener.Deactivate();
            base.SubscriptionsInactive();
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            var list = this.source as System.Collections.IList;
            var priorityList = this.prioritySource as System.Collections.IList;
            if (list != null && list.IsReadOnly)
                return false;
            if (priorityList != null && priorityList.IsReadOnly)
                return false;
            return !this.IsReadOnly;
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return Math.Max(this.source.Count, this.prioritySource.Count);
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            if (index < this.prioritySource.Count)
                return this.prioritySource[index];
            return this.source[index];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            if (index < this.prioritySource.Count)
            {
                using (this.priorityListener.Pause())
                {
                    this.prioritySource[index] = item;
                }
            }
            else
            {
                using (this.listener.Pause())
                {
                    this.source[index] = item;
                }
            }
        }
    }
}

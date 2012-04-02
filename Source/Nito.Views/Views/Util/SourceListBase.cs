using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// A view that is based on a source list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public class SourceListBase<T> : ListBase<T>, CollectionChangedListener<T>.IResponder
    {
        /// <summary>
        /// The source list.
        /// </summary>
        protected readonly IList<T> source;

        /// <summary>
        /// The listener for the source list.
        /// </summary>
        protected readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceListBase&lt;T&gt;"/> class over the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        public SourceListBase(IList<T> source)
        {
            this.source = source;
            this.listener = CollectionChangedListener<T>.Create(source, this);
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only iff its source list is read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.source.IsReadOnly; }
        }

        void CollectionChangedListener<T>.IResponder.Added(int index, T item)
        {
            this.SourceCollectionAdded(index, item);
        }

        void CollectionChangedListener<T>.IResponder.Removed(int index, T item)
        {
            this.SourceCollectionRemoved(index, item);
        }

        void CollectionChangedListener<T>.IResponder.Replaced(int index, T oldItem, T newItem)
        {
            this.SourceCollectionReplaced(index, oldItem, newItem);
        }

        void CollectionChangedListener<T>.IResponder.Reset()
        {
            this.SourceCollectionReset();
        }

        /// <summary>
        /// A notification that the source collection has added an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected virtual void SourceCollectionAdded(int index, T item)
        {
            this.CreateNotifier().Added(index, item);
        }

        /// <summary>
        /// A notification that the source collection has removed an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        protected virtual void SourceCollectionRemoved(int index, T item)
        {
            this.CreateNotifier().Removed(index, item);
        }

        /// <summary>
        /// A notification that the source collection has replaced an item. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected virtual void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Replaced(index, oldItem, newItem);
        }

        /// <summary>
        /// A notification that the source collection has changed significantly. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        protected virtual void SourceCollectionReset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that there is at least one <see cref="CollectionChanged"/> or <see cref="PropertyChanged"/> subscription active. This implementation activates the source listener.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="CollectionChanged"/> nor <see cref="PropertyChanged"/> subscriptions active. This implementation deactivates the source listener.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener.Deactivate();
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="Add"/>, <see cref="Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected override bool CanUpdateCollection()
        {
            var list = this.source as System.Collections.IList;
            if (list != null)
                return !list.IsFixedSize;
            return !this.source.IsReadOnly;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            var list = this.source as System.Collections.IList;
            if (list != null)
                return !list.IsReadOnly;
            return !this.source.IsReadOnly;
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
        {
            this.source.Clear();
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.source.Count;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[index];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            this.source[index] = item;
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            this.source.Insert(index, item);
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            this.source.RemoveAt(index);
        }
    }
}

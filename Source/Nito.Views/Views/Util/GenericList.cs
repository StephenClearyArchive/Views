using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Wraps a non-generic source list, treating it as a generic list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public sealed class GenericList<T> : ListBase<T>, CollectionChangedListener<T>.IResponder
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
        /// Initializes a new instance of the <see cref="GenericList&lt;T&gt;"/> class with the specified source.
        /// </summary>
        /// <param name="source">The non-generic source list to wrap.</param>
        public GenericList(System.Collections.IList source)
        {
            Contract.Requires(source != null);
            this.source = source;
            this.listener = CollectionChangedListener<T>.Create(source, this);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source != null);
        }

        void CollectionChangedListener<T>.IResponder.Added(int index, T item)
        {
            this.CreateNotifier().Added(index, item);
        }

        void CollectionChangedListener<T>.IResponder.Removed(int index, T item)
        {
            this.CreateNotifier().Removed(index, item);
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

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="ICollection{T}.Add"/>, <see cref="ICollection{T}.Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected override bool CanUpdateCollection()
        {
            return !this.source.IsFixedSize;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return !this.source.IsReadOnly;
        }

        /// <summary>
        /// Pauses all notification listeners for source collections. Returns a disposable that will resume the listeners when disposed.
        /// </summary>
        /// <returns>A disposable that will resume the listeners when disposed.</returns>
        protected override IDisposable PauseListeners()
        {
            return this.listener.Pause();
        }

        /// <summary>
        /// Removes all items from this list.
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
            return (T)this.source[index];
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A holder class for constant event args.
    /// </summary>
    public static class CollectionChangedNotifier
    {
        /// <summary>
        /// A <see cref="PropertyChangedEventArgs"/> for the "Count" property.
        /// </summary>
        public readonly static PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs("Count");

        /// <summary>
        /// A <see cref="PropertyChangedEventArgs"/> for the "Item[]" property.
        /// </summary>
        public readonly static PropertyChangedEventArgs ItemsPropertyChangedEventArgs = new PropertyChangedEventArgs("Item[]");
    }
    
    /// <summary>
    /// A type that notifies listeners about collection changes.
    /// </summary>
    public struct CollectionChangedNotifier<T>
    {
        /// <summary>
        /// The instance that initiates the events. If this is <c>null</c>, then there are no listeners.
        /// </summary>
        private object sender;

        /// <summary>
        /// A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        private NotifyCollectionChangedEventHandler collectionHandler;

        /// <summary>
        /// Initializes an instance of the <see cref="CollectionChangedNotifier"/> struct.
        /// </summary>
        /// <param name="sender">The instance that initiates the events.</param>
        /// <param name="collectionHandler">A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.</param>
        public CollectionChangedNotifier(object sender, NotifyCollectionChangedEventHandler collectionHandler)
        {
            Contract.Requires(sender != null);
            this.sender = (collectionHandler == null) ? null : sender;
            this.collectionHandler = collectionHandler;
        }

        /// <summary>
        /// Returns a value indicating whether the processing code should capture original item values.
        /// </summary>
        /// <returns>A value indicating whether the processing code should capture original item values.</returns>
        public bool CaptureItems()
        {
            return (this.collectionHandler != null);
        }

        /// <summary>
        /// Notifies listeners that an item was added.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public void Added(int index, T item)
        {
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Notifies listeners that an item was removed.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        public void Removed(int index, T oldItem)
        {
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        /// <summary>
        /// Notifies listeners that an item was replaced with a new item.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public void Replaced(int index, T oldItem, T newItem)
        {
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        /// <summary>
        /// Notifies listeners that the entire collection has changed.
        /// </summary>
        public void Reset()
        {
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

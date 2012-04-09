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
        /// A snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        private PropertyChangedEventHandler propertyHandler;

        /// <summary>
        /// Initializes an instance of the <see cref="CollectionChangedNotifier"/> struct.
        /// </summary>
        /// <param name="sender">The instance that initiates the events.</param>
        /// <param name="collectionHandler">A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.</param>
        /// <param name="propertyHandler">A snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.</param>
        /// <returns>An instance of <see cref="CollectionChangedNotifier"/> class, or <c>null</c> if there are no event handlers.</returns>
        public CollectionChangedNotifier(object sender, NotifyCollectionChangedEventHandler collectionHandler, PropertyChangedEventHandler propertyHandler)
        {
            Contract.Requires(sender != null);
            if (collectionHandler == null && propertyHandler == null)
            {
                this.sender = null;
                this.collectionHandler = null;
                this.propertyHandler = null;
            }
            else
            {
                this.sender = sender;
                this.collectionHandler = collectionHandler;
                this.propertyHandler = propertyHandler;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the processing code should capture original item values.
        /// </summary>
        /// <returns>A value indicating whether the processing code should capture original item values.</returns>
        public bool CaptureItems()
        {
            return (this.collectionHandler != null);
        }

        private void InvokePropertyHandler()
        {
            if (this.propertyHandler != null)
            {
                this.propertyHandler(this.sender, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                this.propertyHandler(this.sender, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// Notifies listeners that an item was added.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public void Added(int index, T item)
        {
            if (this.sender == null)
                return;
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            this.InvokePropertyHandler();
        }

        /// <summary>
        /// Notifies listeners that an item was removed.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        public void Removed(int index, T oldItem)
        {
            if (this.sender == null)
                return;
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
            this.InvokePropertyHandler();
        }

        /// <summary>
        /// Notifies listeners that an item was replaced with a new item.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public void Replaced(int index, T oldItem, T newItem)
        {
            if (this.sender == null)
                return;
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            this.InvokePropertyHandler();
        }

        /// <summary>
        /// Notifies listeners that the entire collection has changed.
        /// </summary>
        public void Reset()
        {
            if (this.sender == null)
                return;
            if (this.collectionHandler != null)
                this.collectionHandler(this.sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.InvokePropertyHandler();
        }
    }
}

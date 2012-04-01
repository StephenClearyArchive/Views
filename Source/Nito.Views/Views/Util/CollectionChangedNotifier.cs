using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// A type that notifies listeners about collection changes. It is normal for instances of this class to be <c>null</c>.
    /// </summary>
    public sealed class CollectionChangedNotifier
    {
        /// <summary>
        /// A <see cref="PropertyChangedEventArgs"/> for the "Count" property.
        /// </summary>
        public readonly static PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs("Count");

        /// <summary>
        /// A <see cref="PropertyChangedEventArgs"/> for the "Item[]" property.
        /// </summary>
        public readonly static PropertyChangedEventArgs ItemsPropertyChangedEventArgs = new PropertyChangedEventArgs("Item[]");

        /// <summary>
        /// The instance that initiates the events.
        /// </summary>
        private readonly object sender;

        /// <summary>
        /// A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        private readonly NotifyCollectionChangedEventHandler collectionHandler;

        /// <summary>
        /// A snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        private readonly PropertyChangedEventHandler propertyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedNotifier"/> class.
        /// </summary>
        /// <param name="sender">The instance that initiates the events.</param>
        /// <param name="collectionHandler">A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.</param>
        /// <param name="propertyHandler">A snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.</param>
        public CollectionChangedNotifier(object sender, NotifyCollectionChangedEventHandler collectionHandler, PropertyChangedEventHandler propertyHandler)
        {
            this.sender = sender;
            this.collectionHandler = collectionHandler;
            this.propertyHandler = propertyHandler;
        }

        /// <summary>
        /// Gets the instance that initiates the events.
        /// </summary>
        public object Sender
        {
            get { return this.sender; }
        }

        /// <summary>
        /// Gets a snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        public NotifyCollectionChangedEventHandler CollectionHandler
        {
            get { return this.collectionHandler; }
        }

        /// <summary>
        /// Gets a snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.
        /// </summary>
        public PropertyChangedEventHandler PropertyHandler
        {
            get { return this.propertyHandler; }
        }

        /// <summary>
        /// Creates an instance of <see cref="CollectionChangedNotifier"/> class, or <c>null</c> if there are no event handlers.
        /// </summary>
        /// <param name="sender">The instance that initiates the events.</param>
        /// <param name="collectionHandler">A snapshot of the <c>CollectionChanged</c> event handler. This may be <c>null</c>.</param>
        /// <param name="propertyHandler">A snapshot of the <c>PropertyChanged</c> event handler. This may be <c>null</c>.</param>
        /// <returns>An instance of <see cref="CollectionChangedNotifier"/> class, or <c>null</c> if there are no event handlers.</returns>
        public static CollectionChangedNotifier Create(object sender, NotifyCollectionChangedEventHandler collectionHandler, PropertyChangedEventHandler propertyHandler)
        {
            if (collectionHandler == null && propertyHandler == null)
                return null;
            return new CollectionChangedNotifier(sender, collectionHandler, propertyHandler);
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CollectionChangedNotifier"/> class. These methods work for <c>null</c> instances.
    /// </summary>
    public static class CollectionChangedNotifierExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the processing code should capture original item values. This is <c>true</c> if <c>CollectionChanged</c> is not <c>null</c>.
        /// </summary>
        /// <param name="notifier">The notifier. This may be <c>null</c>.</param>
        /// <returns>A value indicating whether the processing code should capture original item values.</returns>
        public static bool CaptureItems(this CollectionChangedNotifier notifier)
        {
            if (notifier == null)
                return false;
            if (notifier.CollectionHandler == null)
                return false;
            return true;
        }

        /// <summary>
        /// Notifies listeners that an item was added.
        /// </summary>
        /// <param name="notifier">The notifier. This may be <c>null</c>.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public static void Added(this CollectionChangedNotifier notifier, int index, object item)
        {
            if (notifier == null)
                return;
            if (notifier.CollectionHandler != null)
                notifier.CollectionHandler(notifier.Sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            if (notifier.PropertyHandler != null)
            {
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// Notifies listeners that an item was removed.
        /// </summary>
        /// <param name="notifier">The notifier. This may be <c>null</c>.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        public static void Removed(this CollectionChangedNotifier notifier, int index, object oldItem)
        {
            if (notifier == null)
                return;
            if (notifier.CollectionHandler != null)
                notifier.CollectionHandler(notifier.Sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
            if (notifier.PropertyHandler != null)
            {
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// Notifies listeners that an item was replaced with a new item.
        /// </summary>
        /// <param name="notifier">The notifier. This may be <c>null</c>.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public static void Replaced(this CollectionChangedNotifier notifier, int index, object oldItem, object newItem)
        {
            if (notifier == null)
                return;
            if (notifier.CollectionHandler != null)
                notifier.CollectionHandler(notifier.Sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            if (notifier.PropertyHandler != null)
            {
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// Notifies listeners that the entire collection has changed.
        /// </summary>
        /// <param name="notifier">The notifier. This may be <c>null</c>.</param>
        public static void Reset(this CollectionChangedNotifier notifier)
        {
            if (notifier == null)
                return;
            if (notifier.CollectionHandler != null)
                notifier.CollectionHandler(notifier.Sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            if (notifier.PropertyHandler != null)
            {
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                notifier.PropertyHandler(notifier.Sender, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A type that listens to collection changes. Listening may be activated or deactivated (which actually unsubscribes). It is normal for instances of this class to be <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of element contained in the collection.</typeparam>
    public sealed class CollectionChangedListener<T>
    {
        /// <summary>
        /// The collection to monitor.
        /// </summary>
        private readonly INotifyCollectionChanged collection;

        /// <summary>
        /// The subscription to <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        private readonly NotifyCollectionChangedEventHandler subscription;

        /// <summary>
        /// The callback object.
        /// </summary>
        private readonly ICollectionChangedResponder<T> responder;

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionChangedListener{T}"/> class. The instance is initially inactive.
        /// </summary>
        /// <param name="collection">The collection to monitor.</param>
        /// <param name="responder">The callback object.</param>
        private CollectionChangedListener(INotifyCollectionChanged collection, ICollectionChangedResponder<T> responder)
        {
            Contract.Requires(collection != null);
            Contract.Requires(responder != null);
            this.collection = collection;
            this.subscription = new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.responder = responder;
        }

        /// <summary>
        /// Gets the collection to monitor.
        /// </summary>
        internal INotifyCollectionChanged Collection
        {
            get
            {
                Contract.Ensures(Contract.Result<INotifyCollectionChanged>() != null);
                return this.collection;
            }
        }

        /// <summary>
        /// Gets the subscription object, which may or may not actually be subscribed.
        /// </summary>
        internal NotifyCollectionChangedEventHandler Subscription
        {
            get
            {
                Contract.Ensures(Contract.Result<NotifyCollectionChangedEventHandler>() != null);
                return this.subscription;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.collection != null);
            Contract.Invariant(this.subscription != null);
            Contract.Invariant(this.responder != null);
        }

        /// <summary>
        /// Handles the collection's <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="sender">The collection that changed.</param>
        /// <param name="args">How the collection changed.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewStartingIndex != -1 &&
                args.NewItems != null && args.NewItems.Count == 1 && args.NewItems[0] is T)
            {
                this.responder.Added(this.collection, args.NewStartingIndex, (T)args.NewItems[0]);
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldStartingIndex != -1 &&
                args.OldItems != null && args.OldItems.Count == 1 && args.OldItems[0] is T)
            {
                this.responder.Removed(this.collection, args.OldStartingIndex, (T)args.OldItems[0]);
            }
            else if (args.Action == NotifyCollectionChangedAction.Replace && args.NewStartingIndex != -1 &&
                args.NewStartingIndex == args.OldStartingIndex && args.NewItems != null && args.NewItems.Count == 1 &&
                args.OldItems != null && args.OldItems.Count == 1 && args.NewItems[0] is T && args.OldItems[0] is T)
            {
                this.responder.Replaced(this.collection, args.NewStartingIndex, (T)args.OldItems[0], (T)args.NewItems[0]);
            }
            else
            {
                this.responder.Reset(this.collection);
            }
        }

        /// <summary>
        /// Creates a new <see cref="CollectionChangedListener{T}"/> to monitor the specified collection. May return <c>null</c>.
        /// </summary>
        /// <param name="collection">The collection to monitor.</param>
        /// <param name="responder">The callback object.</param>
        /// <returns>A new <see cref="CollectionChangedListener{T}"/> or <c>null</c>.</returns>
        public static CollectionChangedListener<T> Create(object collection, ICollectionChangedResponder<T> responder)
        {
            Contract.Requires(collection != null);
            Contract.Requires(responder != null);
            if (!WillCreate(collection))
                return null;
            return new CollectionChangedListener<T>(collection as INotifyCollectionChanged, responder);
        }

        /// <summary>
        /// Returns a value indicating whether <see cref="Create"/> will return a new instance.
        /// </summary>
        /// <param name="collection">The collection to monitor.</param>
        /// <returns>A value indicating whether <see cref="Create"/> will return a new instance.</returns>
        public static bool WillCreate(object collection)
        {
            Contract.Requires(collection != null);
            var supportedCollection = collection as INotifyCollectionChanged;
            if (supportedCollection == null)
                return false;
            var canNotifyCollectionChanged = collection as ICanNotifyCollectionChanged;
            if (canNotifyCollectionChanged != null && !canNotifyCollectionChanged.CanNotifyCollectionChanged)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CollectionChangedListener{T}"/> class. These methods work for <c>null</c> instances.
    /// </summary>
    public static class CollectionChangedListenerExtensions
    {
        /// <summary>
        /// Activates a listener, subscribing to <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the collection.</typeparam>
        /// <param name="listener">The listener to activate. This may be <c>null</c>.</param>
        public static void Activate<T>(this CollectionChangedListener<T> listener)
        {
            if (listener == null)
                return;
            listener.Collection.CollectionChanged += listener.Subscription;
        }

        /// <summary>
        /// Deactivates a listener, unsubscribing from <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the collection.</typeparam>
        /// <param name="listener">The listener to deactivate. This may be <c>null</c>.</param>
        public static void Deactivate<T>(this CollectionChangedListener<T> listener)
        {
            if (listener == null)
                return;
            listener.Collection.CollectionChanged -= listener.Subscription;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A type that listens to collection changes. Listening may be activated or deactivated (which actually unsubscribes), or it may be temporarily paused (which does not unsubscribe). It is normal for instances of this class to be <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of element contained in the collection.</typeparam>
    public sealed class CollectionChangedListener<T>
    {
        /// <summary>
        /// The callbacks for <see cref="CollectionChangedListener{T}"/>
        /// </summary>
        public interface IResponder
        {
            /// <summary>
            /// The collection added an item.
            /// </summary>
            /// <param name="index">The index of the new item.</param>
            /// <param name="item">The item that was added.</param>
            void Added(int index, T item);

            /// <summary>
            /// The collection removed an item.
            /// </summary>
            /// <param name="index">The index of the removed item.</param>
            /// <param name="item">The item that was removed.</param>
            void Removed(int index, T item);

            /// <summary>
            /// The collection replaced an item.
            /// </summary>
            /// <param name="index">The index of the item that changed.</param>
            /// <param name="oldItem">The old item.</param>
            /// <param name="newItem">The new item.</param>
            void Replaced(int index, T oldItem, T newItem);

            /// <summary>
            /// The collection has undergone significant changes.
            /// </summary>
            void Reset();
        }

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
        private readonly IResponder responder;

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionChangedListener{T}"/> class. The instance is initially inactive.
        /// </summary>
        /// <param name="collection">The collection to monitor.</param>
        /// <param name="responder">The callback object.</param>
        private CollectionChangedListener(INotifyCollectionChanged collection, IResponder responder)
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
        public INotifyCollectionChanged Collection
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
        public NotifyCollectionChangedEventHandler Subscription
        {
            get
            {
                Contract.Ensures(Contract.Result<NotifyCollectionChangedEventHandler>() != null);
                return this.subscription;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the listener is paused.
        /// </summary>
        public bool Paused { get; set; }

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
            if (this.Paused)
                return;
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewStartingIndex != -1 &&
                args.NewItems != null && args.NewItems.Count == 1 && args.NewItems[0] is T)
            {
                this.responder.Added(args.NewStartingIndex, (T)args.NewItems[0]);
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldStartingIndex != -1 &&
                args.OldItems != null && args.OldItems.Count == 1 && args.OldItems[0] is T)
            {
                this.responder.Removed(args.OldStartingIndex, (T)args.OldItems[0]);
            }
            else if (args.Action == NotifyCollectionChangedAction.Replace && args.NewStartingIndex != -1 &&
                args.NewStartingIndex == args.OldStartingIndex && args.NewItems != null && args.NewItems.Count == 1 &&
                args.OldItems != null && args.OldItems.Count == 1 && args.NewItems[0] is T && args.OldItems[0] is T)
            {
                this.responder.Replaced(args.NewStartingIndex, (T)args.OldItems[0], (T)args.NewItems[0]);
            }
            else
            {
                this.responder.Reset();
            }
        }

        /// <summary>
        /// Creates a new <see cref="CollectionChangedListener{T}"/> to monitor the specified collection. Returns <c>null</c> if <paramref name="collection"/> does not implement <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        /// <param name="collection">The collection to monitor.</param>
        /// <param name="responder">The callback object.</param>
        /// <returns>A new <see cref="CollectionChangedListener{T}"/> or <c>null</c>.</returns>
        public static CollectionChangedListener<T> Create(object collection, IResponder responder)
        {
            Contract.Requires(collection != null);
            Contract.Requires(responder != null);
            var supportedCollection = collection as INotifyCollectionChanged;
            if (supportedCollection == null)
                return null;
            return new CollectionChangedListener<T>(supportedCollection, responder);
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

        /// <summary>
        /// Pauses a listener. Returns a disposable object (possibly <c>null</c>), which resumes the listener when disposed.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the collection.</typeparam>
        /// <param name="listener">The listener to pause. This may be <c>null</c>.</param>
        /// <returns>A disposable object (possibly <c>null</c>), which resumes the listener when disposed.</returns>
        public static IDisposable Pause<T>(this CollectionChangedListener<T> listener)
        {
            if (listener == null)
                return null;
            listener.Paused = true;
            return new Resumer<T>(listener);
        }

        /// <summary>
        /// A disposable type which resumes a listener when it is disposed.
        /// </summary>
        /// <typeparam name="T">The type of element contained in the collection.</typeparam>
        private sealed class Resumer<T> : IDisposable
        {
            /// <summary>
            /// The listener to resume. This may be <c>null</c>.
            /// </summary>
            private CollectionChangedListener<T> listener;

            /// <summary>
            /// Creates a new instance of the <see cref="Resumer{T}"/> class to resume the specified listener.
            /// </summary>
            /// <param name="listener">The listener to resume. This may not be <c>null</c>.</param>
            public Resumer(CollectionChangedListener<T> listener)
            {
                Contract.Requires(listener != null);
                this.listener = listener;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.listener != null);
            }

            /// <summary>
            /// Disposes the resumer, which resumes the listener (if it has not already done so).
            /// </summary>
            void IDisposable.Dispose()
            {
                if (this.listener == null)
                    return;
                this.listener.Paused = false;
                this.listener = null;
            }
        }
    }
}

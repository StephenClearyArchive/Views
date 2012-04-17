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
    /// Provides common implementations of some view methods.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public abstract class MutableViewBase<T> : ViewBase<T>, ICanNotifyCollectionChanged
    {
        /// <summary>
        /// Backing field for <see cref="CollectionChanged"/>.
        /// </summary>
        private NotifyCollectionChangedEventHandler collectionChanged;

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public abstract bool CanNotifyCollectionChanged { get; }

        /// <summary>
        /// Notifies listeners of changes in the view. This may NOT be accessed by multiple threads.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (!this.CanNotifyCollectionChanged)
                    return;
                bool subscriptionsActivated = (this.collectionChanged == null);
                this.collectionChanged += value;
                if (subscriptionsActivated)
                    this.SubscriptionsActive();
            }

            remove
            {
                if (!this.CanNotifyCollectionChanged)
                    return;
                this.collectionChanged -= value;
                if (this.collectionChanged == null)
                    this.SubscriptionsInactive();
            }
        }

        /// <summary>
        /// Creates a <see cref="CollectionChangedNotifier"/> for notifying of changes to this collection. The return value may be <c>null</c>.
        /// </summary>
        /// <returns>A <see cref="CollectionChangedNotifier"/> for notifying of changes to this collection.</returns>
        protected CollectionChangedNotifier<T> CreateNotifier()
        {
            return new CollectionChangedNotifier<T>(this, this.collectionChanged);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="CollectionChanged"/> subscription active.
        /// </summary>
        protected abstract void SubscriptionsActive();

        /// <summary>
        /// A notification that there are no <see cref="CollectionChanged"/> subscriptions active.
        /// </summary>
        protected abstract void SubscriptionsInactive();
    }
}

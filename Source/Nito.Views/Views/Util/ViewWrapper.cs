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
    /// Provides implementations of common interfaces for views.
    /// </summary>
    /// <typeparam name="T">The type of element contained observed by the view.</typeparam>
    public sealed class ViewWrapper<T> : IList<T>, System.Collections.IList, IView<T>, INotifyCollectionChanged, INotifyPropertyChanged, ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The wrapped view.
        /// </summary>
        private readonly IView<T> view;

        /// <summary>
        /// The listener for the wrapped view.
        /// </summary>
        private readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// Backing field for <see cref="System.Collections.ICollection.SyncRoot"/>.
        /// </summary>
        private readonly object syncRoot;

        /// <summary>
        /// Backing field for <see cref="CollectionChanged"/>.
        /// </summary>
        private NotifyCollectionChangedEventHandler collectionChanged;

        /// <summary>
        /// Backing field for <see cref="PropertyChanged"/>.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewWrapper&lt;T&gt;"/> class around the specified view.
        /// </summary>
        /// <param name="view">The wrapped view.</param>
        public ViewWrapper(IView<T> view)
        {
            this.view = view;
            this.listener = CollectionChangedListener<T>.Create(view, this);
            this.syncRoot = new object();
        }

        /// <summary>
        /// Gets the wrapped view.
        /// </summary>
        public IView<T> View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Notifies listeners of changes in the view. This may NOT be accessed by multiple threads.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                bool subscriptionsActivated = (this.collectionChanged == null && this.propertyChanged == null);
                this.collectionChanged += value;
                if (subscriptionsActivated)
                    this.SubscriptionsActive();
            }

            remove
            {
                this.collectionChanged -= value;
                if (this.collectionChanged == null && this.propertyChanged == null)
                    this.SubscriptionsInactive();
            }
        }

        /// <summary>
        /// Notifies listeners of changes in the view's properties. This may NOT be accessed by multiple threads.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                bool subscriptionsActivated = (this.collectionChanged == null && this.propertyChanged == null);
                this.propertyChanged += value;
                if (subscriptionsActivated)
                    this.SubscriptionsActive();
            }

            remove
            {
                this.propertyChanged -= value;
                if (this.collectionChanged == null && this.propertyChanged == null)
                    this.SubscriptionsInactive();
            }
        }

        private void SubscriptionsActive()
        {
            this.listener.Activate();
        }

        private void SubscriptionsInactive()
        {
            this.listener.Deactivate();
        }

        private CollectionChangedNotifier<T> CreateNotifier()
        {
            return new CollectionChangedNotifier<T>(this, this.collectionChanged);
        }

        private void NotifyPropertyChanged(CollectionChangedNotifier<T> notifier, bool countChanged = true)
        {
            if (notifier.CaptureItems() && this.propertyChanged != null)
            {
                if (countChanged)
                    this.propertyChanged(this, CollectionChangedNotifier.CountPropertyChangedEventArgs);
                this.propertyChanged(this, CollectionChangedNotifier.ItemsPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public void Added(INotifyCollectionChanged collection, int index, T item)
        {
            var notifier = this.CreateNotifier();
            notifier.Added(index, item);
            this.NotifyPropertyChanged(notifier);
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            var notifier = this.CreateNotifier();
            notifier.Removed(index, item);
            this.NotifyPropertyChanged(notifier);
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            var notifier = this.CreateNotifier();
            notifier.Replaced(index, oldItem, newItem);
            this.NotifyPropertyChanged(notifier, false);
        }

        /// <summary>
        /// A notification that the source collection has changed significantly. This implementation passes along the notification to the notifier for this view.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public void Reset(INotifyCollectionChanged collection)
        {
            var notifier = this.CreateNotifier();
            notifier.Reset();
            this.NotifyPropertyChanged(notifier);
        }

        int IView<T>.Count
        {
            get { return this.view.Count; }
        }

        int ICollection<T>.Count
        {
            get { return this.view.Count; }
        }

        int System.Collections.ICollection.Count
        {
            get { return this.view.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return true; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return true; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return this.syncRoot; }
        }

        T IList<T>.this[int index]
        {
            get { return this.view[index]; }
            set { throw this.NotSupported(); }
        }

        T IView<T>.this[int index]
        {
            get { return this.view[index]; }
        }

        object System.Collections.IList.this[int index]
        {
            get { return this.view[index]; }
            set { throw this.NotSupported(); }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.syncRoot != null);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw this.NotSupported();
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            throw this.NotSupported();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw this.NotSupported();
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            throw this.NotSupported();
        }

        void ICollection<T>.Clear()
        {
            throw this.NotSupported();
        }

        void System.Collections.IList.Clear()
        {
            throw this.NotSupported();
        }

        int IList<T>.IndexOf(T item)
        {
            return this.view.FirstIndexOf(item);
        }

        int System.Collections.IList.IndexOf(object value)
        {
            if (!this.ObjectIsT(value))
            {
                return -1;
            }

            return this.view.FirstIndexOf((T)value);
        }

        void ICollection<T>.Add(T item)
        {
            throw this.NotSupported();
        }

        int System.Collections.IList.Add(object value)
        {
            throw this.NotSupported();
        }

        bool ICollection<T>.Contains(T item)
        {
            return (this.view as IEnumerable<T>).Contains(item);
        }

        bool System.Collections.IList.Contains(object value)
        {
            if (!this.ObjectIsT(value))
            {
                return false;
            }

            return (this.view as IEnumerable<T>).Contains((T)value);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            int count = this.view.Count;
            for (int i = 0; i != count; ++i)
            {
                array[arrayIndex + i] = this.view[i];
            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            int count = this.view.Count;
            for (int i = 0; i != count; ++i)
            {
                array.SetValue(this.view[i], index + i);
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw this.NotSupported();
        }

        void System.Collections.IList.Remove(object value)
        {
            throw this.NotSupported();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (this.view as IEnumerable<T>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this.view as IEnumerable<T>).GetEnumerator();
        }

        /// <summary>
        /// Returns whether or not the type of a given item indicates it is appropriate for storing in this list.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns><c>true</c> if the item is appropriate to store in this list; otherwise, <c>false</c>.</returns>
        private bool ObjectIsT(object item)
        {
            if (item is T)
            {
                return true;
            }

            if (item == null && !typeof(T).IsValueType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an exception stating that the operation is not supported.
        /// </summary>
        /// <returns>An exception stating that the operation is not supported.</returns>
        private Exception NotSupported()
        {
            return new NotSupportedException("This operation is not supported.");
        }
    }
}

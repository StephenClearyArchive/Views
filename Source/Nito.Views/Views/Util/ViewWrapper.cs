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
    public sealed class ViewWrapper<T> : IList<T>, System.Collections.IList, IView<T>, INotifyCollectionChanged // TODO: , INotifyPropertyChanged
    {
        /// <summary>
        /// The wrapped view.
        /// </summary>
        private readonly IView<T> view;

        /// <summary>
        /// Backing field for <see cref="System.Collections.ICollection.SyncRoot"/>.
        /// </summary>
        private readonly object syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewWrapper&lt;T&gt;"/> class around the specified view.
        /// </summary>
        /// <param name="view">The wrapped view.</param>
        public ViewWrapper(IView<T> view)
        {
            this.view = view;
            this.syncRoot = new object();
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        public int Count
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

        /// <summary>
        /// Notifies listeners of changes in the view. This may NOT be accessed by multiple threads.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { (this.view as INotifyCollectionChanged).CollectionChanged += value; }
            remove { (this.view as INotifyCollectionChanged).CollectionChanged -= value; }
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

        public void Clear()
        {
            throw this.NotSupported();
        }

        /// <summary>
        /// Determines the index of a specific item in this list.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>The index of <paramref name="item"/> if found in this list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i != this.Count; ++i)
            {
                if (comparer.Equals(item, this.view[i]))
                    return i;
            }

            return -1;
        }

        int System.Collections.IList.IndexOf(object value)
        {
            if (!this.ObjectIsT(value))
            {
                return -1;
            }

            return this.IndexOf((T)value);
        }

        void ICollection<T>.Add(T item)
        {
            throw this.NotSupported();
        }

        int System.Collections.IList.Add(object value)
        {
            throw this.NotSupported();
        }

        /// <summary>
        /// Determines whether this list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this list; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return this.Contains(item, null); // TODO: ArgumentNullException?
        }

        bool System.Collections.IList.Contains(object value)
        {
            if (!this.ObjectIsT(value))
            {
                return false;
            }

            return this.Contains((T)value);
        }

        /// <summary>
        /// Copies the elements of this list to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from this slice. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "Array is null");
            }

            int count = this.Count;
            for (int i = 0; i != count; ++i)
            {
                array[arrayIndex + i] = this.view[i];
            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "Array is null.");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Multidimensional arrays are not supported.");
            }

            try
            {
                Array.Copy(this.ToArray(), 0, array, 0, this.Count);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Invalid array argument; see inner exception for details.", ex);
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i != count; ++i)
            {
                yield return this.view[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

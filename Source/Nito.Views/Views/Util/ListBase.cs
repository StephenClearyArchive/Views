﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Provides common implementations of some list methods.
    /// </summary>
    /// <typeparam name="T">The type of element contained in the list.</typeparam>
    public abstract class ListBase<T> : IList<T>, System.Collections.IList
    {
        /// <summary>
        /// Backing field for <see cref="ICollection.SyncRoot"/>.
        /// </summary>
        private readonly object syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBase&lt;T&gt;"/> class.
        /// </summary>
        public ListBase()
        {
            this.syncRoot = new object();
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        public virtual int Count
        {
            get
            {
                return this.DoCount();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public virtual bool IsReadOnly
        {
            get { return !(this.CanUpdateCollection() && this.CanUpdateElementValues()); }
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return !this.CanUpdateCollection(); }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return !this.CanUpdateElementValues(); }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return this.syncRoot; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in this list.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// This property is set and the list is read-only.
        /// </exception>
        public virtual T this[int index]
        {
            get
            {
                ListHelper.CheckExistingIndexArgument(this.Count, index);
                return this.DoGetItem(index);
            }

            set
            {
                ListHelper.CheckExistingIndexArgument(this.Count, index);
                this.DoSetItem(index, value);
            }
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                if (!this.ObjectIsT(value))
                {
                    throw this.WrongObjectType();
                }

                this[index] = (T)value;
            }
        }

        /// <summary>
        /// Inserts an item to this list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into this list.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in this list.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public virtual void Insert(int index, T item)
        {
            ListHelper.CheckNewIndexArgument(this.Count, index);
            this.DoInsert(index, item);
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            if (!this.ObjectIsT(value))
            {
                throw this.WrongObjectType();
            }

            this.Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in this list.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public virtual void RemoveAt(int index)
        {
            ListHelper.CheckExistingIndexArgument(this.Count, index);
            this.DoRemoveAt(index);
        }

        /// <summary>
        /// Removes all items from this list.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public abstract void Clear();

        /// <summary>
        /// Determines the index of a specific item in this list.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>The index of <paramref name="item"/> if found in this list; otherwise, -1.</returns>
        public virtual int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i != this.Count; ++i)
            {
                if (comparer.Equals(item, this[i]))
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

        /// <summary>
        /// Adds an item to the end of this list.
        /// </summary>
        /// <param name="item">The object to add to this list.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public virtual void Add(T item)
        {
            this.DoInsert(this.Count, item);
        }

        int System.Collections.IList.Add(object value)
        {
            if (!this.ObjectIsT(value))
            {
                throw this.WrongObjectType();
            }

            this.Add((T)value);
            return this.Count - 1;
        }

        /// <summary>
        /// Determines whether this list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in this list.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this list; otherwise, false.
        /// </returns>
        public virtual bool Contains(T item)
        {
            return this.Contains(item, null);
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
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "Array is null");
            }

            int count = this.Count;
            ListHelper.CheckRangeArguments(array.Length, arrayIndex, count);
            for (int i = 0; i != count; ++i)
            {
                array[arrayIndex + i] = this[i];
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

        /// <summary>
        /// Removes the first occurrence of a specific object from this list.
        /// </summary>
        /// <param name="item">The object to remove from this list.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from this list; otherwise, false. This method also returns false if <paramref name="item"/> is not found in this list.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// This list is read-only.
        /// </exception>
        public virtual bool Remove(T item)
        {
            int index = this.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            this.DoRemoveAt(index);
            return true;
        }

        void System.Collections.IList.Remove(object value)
        {
            if (!this.ObjectIsT(value))
            {
                return;
            }

            this.Remove((T)value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i != count; ++i)
            {
                yield return this.DoGetItem(i);
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
        /// Returns an exception indicating that the type of an item indicates it is not appropriate for storing in this list.
        /// </summary>
        /// <returns>An exception indicating that the type of an item indicates it is not appropriate for storing in this list.</returns>
        protected virtual Exception WrongObjectType()
        {
            return new ArgumentException("Object is not compatible with the type of elements contained in this list.");
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="Add"/>, <see cref="Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected virtual bool CanUpdateCollection()
        {
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected virtual bool CanUpdateElementValues()
        {
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected abstract int DoCount();

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected abstract T DoGetItem(int index);

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected abstract void DoSetItem(int index, T item);

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected abstract void DoInsert(int index, T item);

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected abstract void DoRemoveAt(int index);
    }
}

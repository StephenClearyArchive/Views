using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Concatenates a list of source lists into a single list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class ConcatList<T> : ListBase<T>
    {
        /// <summary>
        /// The sequence of source lists.
        /// </summary>
        private readonly IEnumerable<IList<T>> sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcatList&lt;T&gt;"/> class with the specified source lists.
        /// </summary>
        /// <param name="sources">The source lists to concatenate.</param>
        public ConcatList(IEnumerable<IList<T>> sources)
        {
            this.sources = sources;
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only if any of its source lists are read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.sources.Any(x => x.IsReadOnly); }
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="Add"/>, <see cref="Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected override bool CanUpdateCollection()
        {
            if (this.sources.OfType<System.Collections.IList>().Any(list => list.IsFixedSize))
                return false;
            return !this.IsReadOnly;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            if (this.sources.OfType<System.Collections.IList>().Any(list => list.IsReadOnly))
                return false;
            return !this.IsReadOnly;
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public override void Clear()
        {
            foreach (var source in this.sources)
            {
                source.Clear();
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.sources.Sum(x => x.Count);
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);
            return source[sourceIndex];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);
            source[sourceIndex] = item;
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            IList<T> source;
            int sourceIndex;
            this.FindNewIndex(index, out source, out sourceIndex);
            source.Insert(sourceIndex, item);
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);
            source.RemoveAt(sourceIndex);
        }

        /// <summary>
        /// Finds the source list and its index for accessing a specified concatenated index.
        /// </summary>
        /// <param name="concatIndex">The concatenated index.</param>
        /// <param name="source">On return, holds the source list corresponding to the concatenated index.</param>
        /// <param name="sourceIndex">On return, holds the source list index corresponding to the concatenated index.</param>
        private void FindExistingIndex(int concatIndex, out IList<T> source, out int sourceIndex)
        {
            source = null;
            sourceIndex = concatIndex;
            foreach (var sourceList in this.sources)
            {
                if (sourceIndex < sourceList.Count)
                {
                    source = sourceList;
                    return;
                }

                sourceIndex -= sourceList.Count;
            }
        }

        /// <summary>
        /// Finds the source list and its index for inserting at a specified concatenated index.
        /// </summary>
        /// <param name="concatIndex">The concatenated index at which to insert.</param>
        /// <param name="source">On return, holds the source list corresponding to the concatenated index.</param>
        /// <param name="sourceIndex">On return, holds the source list index corresponding to the concatenated index.</param>
        private void FindNewIndex(int concatIndex, out IList<T> source, out int sourceIndex)
        {
            source = null;
            sourceIndex = concatIndex;
            foreach (var sourceList in this.sources)
            {
                if (sourceIndex <= sourceList.Count)
                {
                    source = sourceList;
                    return;
                }

                sourceIndex -= sourceList.Count;
            }
        }
    }
}

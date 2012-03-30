﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// An indirect list, which provides a layer of indirection for the index values of a source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public sealed class IndirectList<T> : Util.ReadOnlyListBase<T>
    {
        /// <summary>
        /// The redirected index values.
        /// </summary>
        private readonly int[] indices;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectList&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source list. The number of elements in the source list may not change as long as this <see cref="IndirectList{T}"/> is reachable.</param>
        public IndirectList(IList<T> source)
        {
            this.Source = source;
            this.indices = new int[source.Count];
            for (int i = 0; i != this.indices.Length; ++i)
                this.indices[i] = i;
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        public IList<T> Source { get; private set; }

        /// <summary>
        /// Gets the redirected index values. Elements in this list may be set, but not inserted or removed.
        /// </summary>
        public IList<int> Indices
        {
            get { return this.indices; }
        }

        /// <summary>
        /// Returns an indirect comparer which may be used to sort or compare elements in <see cref="Indices"/>, based on a source comparer.
        /// </summary>
        /// <param name="comparer">The source comparer. If this is <c>null</c>, then <see cref="Comparer<T>.Default"/> is used.</param>
        /// <returns>The indirect comparer.</returns>
        public IComparer<int> GetComparer(IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return new Util.AnonymousComparer<int> { Compare = (x, y) => comparer.Compare(this[x], this[y]) };
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            if (this.Source.Count != this.indices.Length)
            {
                throw new InvalidOperationException("Source list count changed when the source is being indirectly referenced");
            }

            return this.indices.Length;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.Source[this.indices[index]];
        }

        /// <summary>
        /// Sets an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            this.Source[this.indices[index]] = item;
        }
    }
}

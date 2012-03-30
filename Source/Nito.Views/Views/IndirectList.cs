using System;
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
        /// The source list.
        /// </summary>
        private readonly IList<T> source;

        /// <summary>
        /// The redirected index values.
        /// </summary>
        private readonly IList<int> indices;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectList&lt;T&gt;"/> class for the given source list, using the given redirected index values.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="indices">The redirected index values. If this is <c>null</c>, then a new list of indices is created matching the current source indices.</param>
        public IndirectList(IList<T> source, IList<int> indices = null)
        {
            this.source = source;
            this.indices = indices ?? Enumerable.Range(0, source.Count).ToList();
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        public IList<T> Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the redirected index values.
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
            return this.indices.Count;
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

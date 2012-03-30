using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Reverses a source list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class ReverseList<T> : ListBase<T>
    {
        /// <summary>
        /// The source list.
        /// </summary>
        private readonly IList<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseList&lt;T&gt;"/> class over the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        public ReverseList(IList<T> source)
        {
            this.source = source;
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only iff its source list is read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.source.IsReadOnly; }
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public override void Clear()
        {
            this.source.Clear();
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.source.Count;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[this.source.Count - index - 1];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            this.source[this.source.Count - index - 1] = item;
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            this.source.Insert(this.source.Count - index, item);
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            this.source.RemoveAt(this.source.Count - index - 1);
        }
    }
}

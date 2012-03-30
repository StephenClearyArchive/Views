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
    public sealed class ReverseList<T> : SourceListBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseList&lt;T&gt;"/> class over the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        public ReverseList(IList<T> source)
            : base(source)
        {
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

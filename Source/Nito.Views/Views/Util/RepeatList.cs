using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// A read-only list that is another list repeated a number of times.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class RepeatList<T> : ReadOnlySourceListBase<T>
    {
        /// <summary>
        /// The number of times the source view is repeated.
        /// </summary>
        private readonly int repeatCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="repeatCount">The number of times the source view is repeated. This must be greater than or equal to <c>0</c>.</param>
        public RepeatList(IList<T> source, int repeatCount)
            : base(source)
        {
            this.repeatCount = repeatCount;
        }

        protected override void SourceCollectionAdded(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        protected override void SourceCollectionRemoved(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.source.Count * repeatCount;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[index % this.source.Count];
        }
    }
}

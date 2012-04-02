using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Projects a source list to a result list, and projects the result list back to the source list.
    /// </summary>
    /// <typeparam name="TSource">The type of object contained in the source list.</typeparam>
    /// <typeparam name="TResult">The type of object contained in the resulting list.</typeparam>
    public sealed class ProjectionList<TSource, TResult> : ListBase<TResult>, CollectionChangedListener<TSource>.IResponder
    {
        /// <summary>
        /// The source list.
        /// </summary>
        private readonly IList<TSource> source;

        /// <summary>
        /// The listener for the source list.
        /// </summary>
        private readonly CollectionChangedListener<TSource> listener;

        /// <summary>
        /// The projection function from source to result.
        /// </summary>
        private readonly Func<TSource, int, TResult> selector;

        /// <summary>
        /// The projection function from result to source.
        /// </summary>
        private readonly Func<TResult, int, TSource> reverseSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteProjectList{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="selector">The projection function from source to result.</param>
        /// <param name="reverseSelector">The projection function from result to source.</param>
        public ProjectionList(IList<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TSource> reverseSelector)
        {
            this.source = source;
            this.selector = selector;
            this.reverseSelector = reverseSelector;
            this.listener = CollectionChangedListener<TSource>.Create(source, this);
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only if its source list is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.source.IsReadOnly; }
        }

        void CollectionChangedListener<TSource>.IResponder.Added(int index, TSource item)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Added(index, this.selector(item, index));
        }

        void CollectionChangedListener<TSource>.IResponder.Removed(int index, TSource item)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Removed(index, this.selector(item, index));
        }

        void CollectionChangedListener<TSource>.IResponder.Replaced(int index, TSource oldItem, TSource newItem)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Replaced(index, this.selector(oldItem, index), this.selector(newItem, index));
        }

        void CollectionChangedListener<TSource>.IResponder.Reset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
        {
            using (this.listener.Pause())
            {
                this.source.Clear();
            }
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
        protected override TResult DoGetItem(int index)
        {
            if (this.selector == null)
                throw this.NotSupported();

            return this.selector(this.source[index], index);
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, TResult item)
        {
            if (this.reverseSelector == null)
                throw this.NotSupported();

            using (this.listener.Pause())
            {
                this.source[index] = this.reverseSelector(item, index);
            }
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, TResult item)
        {
            if (this.reverseSelector == null)
                throw this.NotSupported();

            using (this.listener.Pause())
            {
                this.source.Insert(index, this.reverseSelector(item, index));
            }
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            using (this.listener.Pause())
            {
                this.source.RemoveAt(index);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Projects source lists to a result list, and projects the result list back to the source lists.
    /// </summary>
    /// <typeparam name="TSource0">The type of object contained in source list 0.</typeparam>
    /// <typeparam name="TSource1">The type of object contained in source list 1.</typeparam>
    /// <typeparam name="TResult">The type of object contained in the resulting list.</typeparam>
    public sealed class ProjectionList<TSource0, TSource1, TResult> : ListBase<TResult>
    {
        /// <summary>
        /// A type that forwards changes in individual source collections to its parent list.
        /// </summary>
        private sealed class SourceChangeResponder0 : CollectionChangedListener<TSource0>.IResponder
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionList<TSource0, TSource1, TResult> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="SourceChangeResponder"/> class.
            /// </summary>
            /// <param name="parent">The parent list.</param>
            public SourceChangeResponder0(ProjectionList<TSource0, TSource1, TResult> parent)
            {
                this.parent = parent;
            }

            void CollectionChangedListener<TSource0>.IResponder.Added(int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            void CollectionChangedListener<TSource0>.IResponder.Removed(int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            void CollectionChangedListener<TSource0>.IResponder.Replaced(int index, TSource0 oldItem, TSource0 newItem)
            {
                var selector = this.parent.selector;
                var notifier = this.parent.CreateNotifier();
                if (selector == null)
                    notifier.Reset();
                else
                    notifier.Replaced(index, selector(oldItem, this.parent.source1[index]), selector(newItem, this.parent.source1[index]));
            }

            void CollectionChangedListener<TSource0>.IResponder.Reset()
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// A type that forwards changes in individual source collections to its parent list.
        /// </summary>
        private sealed class SourceChangeResponder1 : CollectionChangedListener<TSource1>.IResponder
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionList<TSource0, TSource1, TResult> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="SourceChangeResponder"/> class.
            /// </summary>
            /// <param name="parent">The parent list.</param>
            public SourceChangeResponder1(ProjectionList<TSource0, TSource1, TResult> parent)
            {
                this.parent = parent;
            }

            void CollectionChangedListener<TSource1>.IResponder.Added(int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            void CollectionChangedListener<TSource1>.IResponder.Removed(int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            void CollectionChangedListener<TSource1>.IResponder.Replaced(int index, TSource1 oldItem, TSource1 newItem)
            {
                var selector = this.parent.selector;
                var notifier = this.parent.CreateNotifier();
                if (selector == null)
                    notifier.Reset();
                else
                    notifier.Replaced(index, selector(this.parent.source0[index], oldItem), selector(this.parent.source0[index], newItem));
            }

            void CollectionChangedListener<TSource1>.IResponder.Reset()
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// Source list 0.
        /// </summary>
        private readonly IList<TSource0> source0;

        /// <summary>
        /// Source list 1.
        /// </summary>
        private readonly IList<TSource1> source1;

        /// <summary>
        /// The listener for source list 0.
        /// </summary>
        private readonly CollectionChangedListener<TSource0> listener0;

        /// <summary>
        /// The listener for source list 1.
        /// </summary>
        private readonly CollectionChangedListener<TSource1> listener1;

        /// <summary>
        /// The projection function from sources to result.
        /// </summary>
        private readonly Func<TSource0, TSource1, TResult> selector;

        /// <summary>
        /// The projection function from result to source.
        /// </summary>
        private readonly Func<TResult, Tuple<TSource0, TSource1>> reverseSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionList{TSource0,TSource1,TResult}"/> class.
        /// </summary>
        /// <param name="source0">Source list 0.</param>
        /// <param name="source1">Source list 1.</param>
        /// <param name="selector">The projection function from sources to result.</param>
        /// <param name="reverseSelector">The projection function from result to sources.</param>
        public ProjectionList(IList<TSource0> source0, IList<TSource1> source1, Func<TSource0, TSource1, TResult> selector, Func<TResult, Tuple<TSource0, TSource1>> reverseSelector)
        {
            this.source0 = source0;
            this.source1 = source1;
            this.selector = selector;
            this.reverseSelector = reverseSelector;
            this.listener0 = CollectionChangedListener<TSource0>.Create(source0, source0 is INotifyCollectionChanged ? new SourceChangeResponder0(this) : null);
            this.listener1 = CollectionChangedListener<TSource1>.Create(source1, source1 is INotifyCollectionChanged ? new SourceChangeResponder1(this) : null);
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only if any of its source lists are read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.source0.IsReadOnly || this.source1.IsReadOnly; }
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
        {
            using (this.listener0.Pause())
            using (this.listener1.Pause())
            {
                this.source0.Clear();
                this.source1.Clear();
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return Math.Min(this.source0.Count, this.source1.Count);
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

            return this.selector(this.source0[index], this.source1[index]);
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

            using (this.listener0.Pause())
            using (this.listener1.Pause())
            {
                var items = this.reverseSelector(item);
                this.source0[index] = items.Item1;
                this.source1[index] = items.Item2;
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

            using (this.listener0.Pause())
            using (this.listener1.Pause())
            {
                var items = this.reverseSelector(item);
                this.source0.Insert(index, items.Item1);
                this.source1.Insert(index, items.Item2);
            }
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            using (this.listener0.Pause())
            using (this.listener1.Pause())
            {
                this.source0.RemoveAt(index);
                this.source1.RemoveAt(index);
            }
        }
    }
}

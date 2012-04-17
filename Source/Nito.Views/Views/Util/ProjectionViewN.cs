

// This file was automatically generated. Do not edit.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Views.Util
{
    /// <summary>
    /// Projects source views to a result view.
    /// </summary>
    /// <typeparam name="TSource0">The type of element observed by source view 0.</typeparam>
    /// <typeparam name="TSource1">The type of element observed by source view 1.</typeparam>
    /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
    public sealed class ProjectionView<TSource0, TSource1, TResult> : MutableViewBase<TResult>
    {
        /// <summary>
        /// A type that forwards changes in individual source collections to its parent view.
        /// </summary>
        private sealed class SourceChangeResponder0 : ICollectionChangedResponder<TSource0>
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionView<TSource0, TSource1, TResult> parent;

            public SourceChangeResponder0(ProjectionView<TSource0, TSource1, TResult> parent)
            {
                Contract.Requires(parent != null);
                this.parent = parent;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.parent != null);
            }

            public void Added(INotifyCollectionChanged collection, int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Removed(INotifyCollectionChanged collection, int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Replaced(INotifyCollectionChanged collection, int index, TSource0 oldItem, TSource0 newItem)
            {
                var selector = this.parent.selector;
                this.parent.CreateNotifier().Replaced(index, selector(oldItem, this.parent.source1[index]), selector(newItem, this.parent.source1[index]));
            }

            public void Reset(INotifyCollectionChanged collection)
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// A type that forwards changes in individual source collections to its parent view.
        /// </summary>
        private sealed class SourceChangeResponder1 : ICollectionChangedResponder<TSource1>
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionView<TSource0, TSource1, TResult> parent;

            public SourceChangeResponder1(ProjectionView<TSource0, TSource1, TResult> parent)
            {
                Contract.Requires(parent != null);
                this.parent = parent;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.parent != null);
            }

            public void Added(INotifyCollectionChanged collection, int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Removed(INotifyCollectionChanged collection, int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Replaced(INotifyCollectionChanged collection, int index, TSource1 oldItem, TSource1 newItem)
            {
                var selector = this.parent.selector;
                this.parent.CreateNotifier().Replaced(index, selector(this.parent.source0[index], oldItem), selector(this.parent.source0[index], newItem));
            }

            public void Reset(INotifyCollectionChanged collection)
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// Source view 0.
        /// </summary>
        private readonly IView<TSource0> source0;

        /// <summary>
        /// The listener for source view 0.
        /// </summary>
        private readonly CollectionChangedListener<TSource0> listener0;

        /// <summary>
        /// Source view 1.
        /// </summary>
        private readonly IView<TSource1> source1;

        /// <summary>
        /// The listener for source view 1.
        /// </summary>
        private readonly CollectionChangedListener<TSource1> listener1;

        /// <summary>
        /// The projection function from sources to result.
        /// </summary>
        private readonly Func<TSource0, TSource1, TResult> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionView{TSource0, TSource1,TResult}"/> class.
        /// </summary>
        /// <param name="source0">Source view 0.</param>
        /// <param name="source1">Source view 1.</param>
        /// <param name="selector">The projection function from sources to result.</param>
        public ProjectionView(IView<TSource0> source0, IView<TSource1> source1, Func<TSource0, TSource1, TResult> selector)
        {
            Contract.Requires(source0 != null);
            Contract.Requires(source1 != null);
            Contract.Requires(selector != null);
            this.source0 = source0;
            this.listener0 = CollectionChangedListener<TSource0>.Create(source0, CollectionChangedListener<TSource0>.WillCreate(source0) ? new SourceChangeResponder0(this) : null);
            this.source1 = source1;
            this.listener1 = CollectionChangedListener<TSource1>.Create(source1, CollectionChangedListener<TSource1>.WillCreate(source1) ? new SourceChangeResponder1(this) : null);
            this.selector = selector;
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return Min(this.source0.Count, this.source1.Count); }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override TResult this[int index]
        {
            get { return this.selector(this.source0[index], this.source1[index]); }
        }

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public override bool CanNotifyCollectionChanged
        {
            get { return (this.source0 as ICanNotifyCollectionChanged).CanNotifyCollectionChanged || (this.source1 as ICanNotifyCollectionChanged).CanNotifyCollectionChanged; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source0 != null);
            Contract.Invariant(this.source1 != null);
            Contract.Invariant(this.selector != null);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="MutableViewBase{T}.CollectionChanged"/> subscription active.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener0.Activate();
            this.listener1.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="MutableViewBase{T}.CollectionChanged"/> subscriptions active.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener0.Deactivate();
            this.listener1.Deactivate();
        }

        private int Min(params int[] counts)
		{
		    return counts.Min();
		}
	}

    /// <summary>
    /// Projects source views to a result view.
    /// </summary>
    /// <typeparam name="TSource0">The type of element observed by source view 0.</typeparam>
    /// <typeparam name="TSource1">The type of element observed by source view 1.</typeparam>
    /// <typeparam name="TSource2">The type of element observed by source view 2.</typeparam>
    /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
    public sealed class ProjectionView<TSource0, TSource1, TSource2, TResult> : MutableViewBase<TResult>
    {
        /// <summary>
        /// A type that forwards changes in individual source collections to its parent view.
        /// </summary>
        private sealed class SourceChangeResponder0 : ICollectionChangedResponder<TSource0>
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionView<TSource0, TSource1, TSource2, TResult> parent;

            public SourceChangeResponder0(ProjectionView<TSource0, TSource1, TSource2, TResult> parent)
            {
                Contract.Requires(parent != null);
                this.parent = parent;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.parent != null);
            }

            public void Added(INotifyCollectionChanged collection, int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Removed(INotifyCollectionChanged collection, int index, TSource0 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Replaced(INotifyCollectionChanged collection, int index, TSource0 oldItem, TSource0 newItem)
            {
                var selector = this.parent.selector;
                this.parent.CreateNotifier().Replaced(index, selector(oldItem, this.parent.source1[index], this.parent.source2[index]), selector(newItem, this.parent.source1[index], this.parent.source2[index]));
            }

            public void Reset(INotifyCollectionChanged collection)
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// A type that forwards changes in individual source collections to its parent view.
        /// </summary>
        private sealed class SourceChangeResponder1 : ICollectionChangedResponder<TSource1>
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionView<TSource0, TSource1, TSource2, TResult> parent;

            public SourceChangeResponder1(ProjectionView<TSource0, TSource1, TSource2, TResult> parent)
            {
                Contract.Requires(parent != null);
                this.parent = parent;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.parent != null);
            }

            public void Added(INotifyCollectionChanged collection, int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Removed(INotifyCollectionChanged collection, int index, TSource1 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Replaced(INotifyCollectionChanged collection, int index, TSource1 oldItem, TSource1 newItem)
            {
                var selector = this.parent.selector;
                this.parent.CreateNotifier().Replaced(index, selector(this.parent.source0[index], oldItem, this.parent.source2[index]), selector(this.parent.source0[index], newItem, this.parent.source2[index]));
            }

            public void Reset(INotifyCollectionChanged collection)
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// A type that forwards changes in individual source collections to its parent view.
        /// </summary>
        private sealed class SourceChangeResponder2 : ICollectionChangedResponder<TSource2>
        {
            /// <summary>
            /// The parent list.
            /// </summary>
            private readonly ProjectionView<TSource0, TSource1, TSource2, TResult> parent;

            public SourceChangeResponder2(ProjectionView<TSource0, TSource1, TSource2, TResult> parent)
            {
                Contract.Requires(parent != null);
                this.parent = parent;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.parent != null);
            }

            public void Added(INotifyCollectionChanged collection, int index, TSource2 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Removed(INotifyCollectionChanged collection, int index, TSource2 item)
            {
                this.parent.CreateNotifier().Reset();
            }

            public void Replaced(INotifyCollectionChanged collection, int index, TSource2 oldItem, TSource2 newItem)
            {
                var selector = this.parent.selector;
                this.parent.CreateNotifier().Replaced(index, selector(this.parent.source0[index], this.parent.source1[index], oldItem), selector(this.parent.source0[index], this.parent.source1[index], newItem));
            }

            public void Reset(INotifyCollectionChanged collection)
            {
                this.parent.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// Source view 0.
        /// </summary>
        private readonly IView<TSource0> source0;

        /// <summary>
        /// The listener for source view 0.
        /// </summary>
        private readonly CollectionChangedListener<TSource0> listener0;

        /// <summary>
        /// Source view 1.
        /// </summary>
        private readonly IView<TSource1> source1;

        /// <summary>
        /// The listener for source view 1.
        /// </summary>
        private readonly CollectionChangedListener<TSource1> listener1;

        /// <summary>
        /// Source view 2.
        /// </summary>
        private readonly IView<TSource2> source2;

        /// <summary>
        /// The listener for source view 2.
        /// </summary>
        private readonly CollectionChangedListener<TSource2> listener2;

        /// <summary>
        /// The projection function from sources to result.
        /// </summary>
        private readonly Func<TSource0, TSource1, TSource2, TResult> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionView{TSource0, TSource1, TSource2,TResult}"/> class.
        /// </summary>
        /// <param name="source0">Source view 0.</param>
        /// <param name="source1">Source view 1.</param>
        /// <param name="source2">Source view 2.</param>
        /// <param name="selector">The projection function from sources to result.</param>
        public ProjectionView(IView<TSource0> source0, IView<TSource1> source1, IView<TSource2> source2, Func<TSource0, TSource1, TSource2, TResult> selector)
        {
            Contract.Requires(source0 != null);
            Contract.Requires(source1 != null);
            Contract.Requires(source2 != null);
            Contract.Requires(selector != null);
            this.source0 = source0;
            this.listener0 = CollectionChangedListener<TSource0>.Create(source0, CollectionChangedListener<TSource0>.WillCreate(source0) ? new SourceChangeResponder0(this) : null);
            this.source1 = source1;
            this.listener1 = CollectionChangedListener<TSource1>.Create(source1, CollectionChangedListener<TSource1>.WillCreate(source1) ? new SourceChangeResponder1(this) : null);
            this.source2 = source2;
            this.listener2 = CollectionChangedListener<TSource2>.Create(source2, CollectionChangedListener<TSource2>.WillCreate(source2) ? new SourceChangeResponder2(this) : null);
            this.selector = selector;
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return Min(this.source0.Count, this.source1.Count, this.source2.Count); }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override TResult this[int index]
        {
            get { return this.selector(this.source0[index], this.source1[index], this.source2[index]); }
        }

        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        public override bool CanNotifyCollectionChanged
        {
            get { return (this.source0 as ICanNotifyCollectionChanged).CanNotifyCollectionChanged || (this.source1 as ICanNotifyCollectionChanged).CanNotifyCollectionChanged || (this.source2 as ICanNotifyCollectionChanged).CanNotifyCollectionChanged; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source0 != null);
            Contract.Invariant(this.source1 != null);
            Contract.Invariant(this.source2 != null);
            Contract.Invariant(this.selector != null);
        }

        /// <summary>
        /// A notification that there is at least one <see cref="MutableViewBase{T}.CollectionChanged"/> subscription active.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener0.Activate();
            this.listener1.Activate();
            this.listener2.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="MutableViewBase{T}.CollectionChanged"/> subscriptions active.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener0.Deactivate();
            this.listener1.Deactivate();
            this.listener2.Deactivate();
        }

        private int Min(params int[] counts)
		{
		    return counts.Min();
		}
	}

}


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

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
        private readonly Func<TSource, TResult> selector;

        /// <summary>
        /// The projection function from result to source.
        /// </summary>
        private readonly Func<TResult, TSource> reverseSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionList{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="selector">The projection function from source to result.</param>
        /// <param name="reverseSelector">The projection function from result to source.</param>
        public ProjectionList(IList<TSource> source, Func<TSource, TResult> selector, Func<TResult, TSource> reverseSelector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(reverseSelector != null);
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source != null);
            Contract.Invariant(this.selector != null);
            Contract.Invariant(this.reverseSelector != null);
        }

        void CollectionChangedListener<TSource>.IResponder.Added(int index, TSource item)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Added(index, this.selector(item));
        }

        void CollectionChangedListener<TSource>.IResponder.Removed(int index, TSource item)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Removed(index, this.selector(item));
        }

        void CollectionChangedListener<TSource>.IResponder.Replaced(int index, TSource oldItem, TSource newItem)
        {
            var notifier = this.CreateNotifier();
            if (this.selector == null)
                notifier.Reset();
            else
                notifier.Replaced(index, this.selector(oldItem), this.selector(newItem));
        }

        void CollectionChangedListener<TSource>.IResponder.Reset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Pauses all notification listeners for source collections. Returns a disposable that will resume the listeners when disposed.
        /// </summary>
        /// <returns>A disposable that will resume the listeners when disposed.</returns>
        protected override IDisposable PauseListeners()
        {
            return base.PauseListeners();
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
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
        protected override TResult DoGetItem(int index)
        {
            if (this.selector == null)
                throw this.NotSupported();

            return this.selector(this.source[index]);
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

            this.source[index] = this.reverseSelector(item);
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

            this.source.Insert(index, this.reverseSelector(item));
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            this.source.RemoveAt(index);
        }
    }
}

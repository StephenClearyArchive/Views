using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Projects a source view to a result view.
    /// </summary>
    /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
    /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
    public sealed class ProjectionView<TSource, TResult> : MutableViewBase<TResult>, ICollectionChangedResponder<TSource>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        private readonly IView<TSource> source;

        /// <summary>
        /// The listener for the source view.
        /// </summary>
        private readonly CollectionChangedListener<TSource> listener;

        /// <summary>
        /// The projection function from source to result.
        /// </summary>
        private readonly Func<TSource, TResult> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionView{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection function from source to result.</param>
        public ProjectionView(IView<TSource> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            this.source = source;
            this.selector = selector;
            this.listener = CollectionChangedListener<TSource>.Create(source, this);
        }

        public override int Count
        {
            get { return this.source.Count; }
        }

        public override TResult this[int index]
        {
            get { return this.selector(this.source[index]); }
        }

        public override bool CanNotifyCollectionChanged
        {
            get { return (this.source as ICanNotifyCollectionChanged).CanNotifyCollectionChanged; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.source != null);
            Contract.Invariant(this.selector != null);
        }

        public void Added(INotifyCollectionChanged collection, int index, TSource item)
        {
            this.CreateNotifier().Added(index, this.selector(item));
        }

        public void Removed(INotifyCollectionChanged collection, int index, TSource item)
        {
            this.CreateNotifier().Removed(index, this.selector(item));
        }

        public void Replaced(INotifyCollectionChanged collection, int index, TSource oldItem, TSource newItem)
        {
            this.CreateNotifier().Replaced(index, this.selector(oldItem), this.selector(newItem));
        }

        public void Reset(INotifyCollectionChanged collection)
        {
            this.CreateNotifier().Reset();
        }
    }
}

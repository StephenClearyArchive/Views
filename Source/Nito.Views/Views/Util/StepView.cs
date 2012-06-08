using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Indexes into a source view using a step size.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class StepView<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The step size to use when traversing the source view.
        /// </summary>
        private readonly int step;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="step">The step size to use when traversing the source view.</param>
        public StepView(IView<T> source, int step)
            : base(source)
        {
            Contract.Requires(step > 0);
            this.step = step;
        }

        /// <summary>
        /// The step size to use when traversing the source view.
        /// </summary>
        public int Step
        {
            get { return this.step; }
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get
            {
                if (this.source.Count == 0)
                    return 0;

                return ((this.source.Count - 1) / this.step) + 1;
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[index * this.step]; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.step > 0);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public override void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem)
        {
            if (index % this.step == 0)
                this.CreateNotifier().Replaced(index / this.step, oldItem, newItem);
        }
    }
}

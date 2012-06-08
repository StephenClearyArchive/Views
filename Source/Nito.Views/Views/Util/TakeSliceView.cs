using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Slices a source view by taking elements up to the point at which a predicate changes its results.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public sealed class TakeSliceView<T> : SliceView<T>
    {
        /// <summary>
        /// The predicate used to slice the view.
        /// </summary>
        private readonly Func<T, bool> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TakeSliceView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="predicate">The predicate used to slice the view.</param>
        public TakeSliceView(IView<T> source, Func<T, bool> predicate)
            : base(source, 0, 0)
        {
            Contract.Requires(source != null);
            Contract.Requires(predicate != null);
            this.predicate = predicate;
            this.ResetSlice();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.offset == 0);
            Contract.Invariant(this.predicate != null);
        }

        /// <summary>
        /// Gets the predicate used to slice the view.
        /// </summary>
        public Func<T, bool> Predicate
        {
            get { return this.predicate; }
        }

        /// <summary>
        /// Recalculates the slice, based on the current source view.
        /// </summary>
        private void ResetSlice()
        {
            this.sliceCount = this.source.FirstIndex(x => !this.predicate(x));
        }

        /// <summary>
        /// Adjusts the slice down, possibly removing elements from this view.
        /// </summary>
        /// <param name="index">The index of the item that does not match the predicate.</param>
        /// <param name="item">The item which does not match the predicate.</param>
        private void AdjustSliceDown(int index, T item)
        {
            Contract.Requires(index < this.sliceCount);
            Contract.Requires(!this.predicate(item));
            var removed = this.sliceCount - index;
            this.sliceCount = index;
            if (removed == 0)
                return;
            if (removed == 1)
                this.CreateNotifier().Removed(index, item);
            else
                this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Adjusts the slice up, possibly adding elements to this view.
        /// </summary>
        private void AdjustSliceUp()
        {
            var added = this.source.Slice(start: this.sliceCount).FirstIndex(x => !this.predicate(x));
            this.sliceCount += added;
            if (added == 0)
                return;
            if (added == 1)
                this.CreateNotifier().Added(this.sliceCount - 1, this.source[this.sliceCount - 1]);
            else
                this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            if (index < this.sliceCount)
            {
                if (!this.predicate(item))
                {
                    this.AdjustSliceDown(index, item);
                }
                else
                {
                    ++this.sliceCount;
                    this.CreateNotifier().Added(index, item);
                }
            }
            else if (index == this.sliceCount && this.predicate(item))
            {
                ++this.sliceCount;
                this.CreateNotifier().Added(index, item);
            }
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            if (index < this.sliceCount)
            {
                --this.sliceCount;
                this.CreateNotifier().Removed(index, item);
            }
            else if (index == this.sliceCount)
            {
                this.AdjustSliceUp();
            }
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
            if (index < this.sliceCount)
            {
                if (!this.predicate(newItem))
                {
                    this.AdjustSliceDown(index, newItem);
                }
                else
                {
                    this.CreateNotifier().Replaced(index, oldItem, newItem);
                }
            }
            else if (index == this.sliceCount)
            {
                this.AdjustSliceUp();
            }
        }

        /// <summary>
        /// A notification that the source collection has changed significantly.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public override void Reset(INotifyCollectionChanged collection)
        {
            this.ResetSlice();
            base.Reset(collection);
        }
    }
}

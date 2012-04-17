using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// A randomly-ordered view over a source view.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public class RandomView<T> : IndirectViewBase<T>
    {
        /// <summary>
        /// The random number generator; when invoked with a value <c>n</c>, this will return a random number in the range [0, n).
        /// </summary>
        private readonly Func<int, int> randomNumberGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomView&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="randomNumberGenerator">The random number generator; when invoked with a value <c>n</c>, this will return a random number in the range [0, n).</param>
        public RandomView(IView<T> source, Func<int, int> randomNumberGenerator)
            : base(source, null)
        {
            Contract.Requires(source != null);
            Contract.Requires(randomNumberGenerator != null);
            this.randomNumberGenerator = randomNumberGenerator;
            this.ResetIndices();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.randomNumberGenerator != null);
        }

        /// <summary>
        /// Creates a new sorted list of redirected indices.
        /// </summary>
        private void ResetIndices()
        {
            var newIndices = DefaultIndices(this.source);
            
            // Randomly shuffle our indices.
            var n = newIndices.Count;
            while (n > 1)
            {
                var k = this.randomNumberGenerator(n);
                Contract.Assert(k >= 0 && k < n);
                --n;
                var temp = newIndices[k];
                newIndices[k] = newIndices[n];
                newIndices[n] = temp;
            }

            this.indices = newIndices;
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        public override void Added(INotifyCollectionChanged collection, int index, T item)
        {
            // Update our existing indexes.
            for (int i = 0; i != this.indices.Count; ++i)
            {
                if (this.indices[i] >= index)
                    ++this.indices[i];
            }

            // Find a random place for the new item.
            var newIndex = this.randomNumberGenerator(this.indices.Count);

            // Insert it there.
            this.indices.Insert(newIndex, index);

            // Notify our listeners that the item was added.
            this.CreateNotifier().Added(newIndex, item);
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        public override void Removed(INotifyCollectionChanged collection, int index, T item)
        {
            // Update our existing indexes.
            int removedIndex = -1;
            for (int i = 0; i != this.indices.Count; ++i)
            {
                if (this.indices[i] > index)
                    --this.indices[i];
                else if (this.indices[i] == index)
                    removedIndex = i;
            }

            // Remove the item.
            this.indices.RemoveAt(removedIndex);

            // Notify our listeners that the item was removed.
            this.CreateNotifier().Removed(removedIndex, item);
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
            var replacedIndex = this.indices.IndexOf(index);
            this.CreateNotifier().Replaced(replacedIndex, oldItem, newItem);
        }

        /// <summary>
        /// A notification that the source collection has changed significantly.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public override void Reset(INotifyCollectionChanged collection)
        {
            this.ResetIndices();
            this.CreateNotifier().Reset();
        }
    }
}

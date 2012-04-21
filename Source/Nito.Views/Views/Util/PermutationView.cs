using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// A view providing permutations of a source view. Permutations are provided in lexicographical order.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public class PermutationView<T> : IndirectViewBase<T>, IViewPermutationGenerator<T>
    {
        /// <summary>
        /// The comparer used to indirectly compare source list elements.
        /// </summary>
        private readonly IndirectComparer<T> indexComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermutationView&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="comparer">The source view element comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        public PermutationView(IView<T> source, IComparer<T> comparer)
            : base(source, null)
        {
            Contract.Requires(source != null);
            this.indexComparer = new IndirectComparer<T>(this, comparer);
            this.ResetToFirstPermutation();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.indexComparer != null);
        }

        /// <summary>
        /// Gets the current permuation of the view. This value is only valid until <see cref="NextPermutation"/> is invoked.
        /// </summary>
        public IView<T> Current
        {
            get { return this; }
        }

        /// <summary>
        /// Creates a new sorted list of redirected indices.
        /// </summary>
        private void ResetToFirstPermutation()
        {
            var newIndices = DefaultIndices(this.source);
            newIndices.Sort(indexComparer);
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
            this.ResetToFirstPermutation();
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
            this.ResetToFirstPermutation();
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
            if (this.indexComparer.Source.Compare(oldItem, newItem) == 0)
            {
                this.CreateNotifier().Replaced(index, oldItem, newItem);
            }
            else
            {
                this.ResetToFirstPermutation();
                this.CreateNotifier().Reset();
            }
        }

        /// <summary>
        /// A notification that the source collection has changed significantly.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        public override void Reset(INotifyCollectionChanged collection)
        {
            this.ResetToFirstPermutation();
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Rearranges the elements in this view to advance to the next permutation. If this method returns <c>false</c>, then there are no more permutations, and this view is reset to the first permutation.
        /// </summary>
        /// <returns><c>true</c> if this view advanced to the next permutation; <c>false</c> if this view reset to the first permutation.</returns>
        public bool NextPermutation()
        {
            var count = this.indices.Count;
            if (count == 0 || count == 1)
                return false;

            // Find the beginning of our current weakly-decreasing sequence. This is our "key" index.
            for (int i = count - 2; i >= 0; --i)
            {
                if (this.indexComparer.Compare(i, i + 1) < 0)
                {
                    // The start and length of the current weakly-decreasing sequence.
                    var start = i + 1;
                    var length = count - start;

                    // Find the last element in the weakly-decreasing sequence that is larger than the element at our key index.
                    for (int j = count - 1; j >= start; --j)
                    {
                        if (this.indexComparer.Compare(i, j) < 0)
                        {
                            // Swap the two elements. This momentarily expands the weakly-decreasing sequence to include the key index.
                            var temp = this.indices[i];
                            this.indices[i] = this.indices[j];
                            this.indices[j] = temp;

                            // Reverse the weakly-decreasing sequence (after the key index). This starts this portion of the permutation over.
                            this.indices.Reverse(start, length);

                            return true;
                        }
                    }

                    Contract.Assert(false);
                }
            }

            // The entire sequence is weakly decreasing, so this permutation is the last permutation.
            return false;
        }
    }
}

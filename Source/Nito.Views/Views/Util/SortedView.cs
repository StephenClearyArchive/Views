using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// A sorted view over a source view.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public class SortedView<T> : IndirectViewBase<T>
    {
        /// <summary>
        /// The comparer used to indirectly compare source list elements.
        /// </summary>
        private readonly IComparer<int> indexComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedView&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="comparer">The source view element comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        public SortedView(IView<T> source, IComparer<T> comparer)
            : base(source, null)
        {
            Contract.Requires(source != null);
            this.indexComparer = this.GetComparer(comparer);
            this.ResetIndices();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.indexComparer != null);
        }

        /// <summary>
        /// Creates a new sorted list of redirected indices.
        /// </summary>
        private void ResetIndices()
        {
            var newIndices = DefaultIndices(this.source);
            this.indices = newIndices;
            newIndices.Sort(indexComparer);
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

            // Find where the new item belongs in our sorted view.
            var newIndex = (this.indices as List<int>).BinarySearch(index, this.indexComparer);
            if (newIndex < 0)
                newIndex = ~newIndex;

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
            // Do a removal followed by an insert.
            // Note that the other source indices do not change, so this code is simpler than an *actual* element removal and insert.
            var removedIndex = this.indices.IndexOf(index);
            this.indices.RemoveAt(removedIndex);
            var newIndex = (this.indices as List<int>).BinarySearch(index, this.indexComparer);
            if (newIndex < 0)
                newIndex = ~newIndex;
            this.indices.Insert(newIndex, index);

            // There is a small chance that we can report this as a "replace", but most likely it'll just be a "reset".
            if (removedIndex == newIndex)
                this.CreateNotifier().Replaced(newIndex, oldItem, newItem);
            else
                this.CreateNotifier().Reset();
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

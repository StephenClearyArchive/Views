using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A filtered list, which provides a filtered view into a source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public sealed class FilteredList<T> : IndirectListBase<T>
    {
        /// <summary>
        /// The filter which determines which elements in the source list are visible in this list.
        /// </summary>
        private readonly Func<T, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredList&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="filter">The filter which determines which elements in the source list are visible in this list.</param>
        public FilteredList(IList<T> source, Func<T, bool> filter)
            : base(source, null)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            this.filter = filter;
            this.ResetIndices();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.filter != null);
            Contract.Invariant(this.indices.Count <= this.source.Count);
        }

        /// <summary>
        /// Creates a new filtered list of redirected indices.
        /// </summary>
        private void ResetIndices()
        {
            var sourceCount = this.source.Count;
            var newIndices = new List<int>(sourceCount);
            for (int i = 0; i != sourceCount; ++i)
            {
                if (this.filter(this.source[i]))
                    newIndices.Add(i);
            }

            this.indices = newIndices;
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            // Update our existing indexes.
            var newIndex = (this.indices as List<int>).BinarySearch(index);
            if (newIndex < 0)
                newIndex = ~newIndex;
            for (int i = newIndex; i != this.indices.Count; ++i)
                ++this.indices[i];

            // If the new item doesn't exist in our view, just ignore it.
            if (!filter(item))
                return;

            // Insert the new item.
            this.indices.Insert(newIndex, index);

            // Notify our listeners that the item was added.
            this.CreateNotifier().Added(newIndex, item);
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
        {
            // Update our existing indexes.
            var removedIndex = (this.indices as List<int>).BinarySearch(index);
            var start = (removedIndex < 0) ? ~removedIndex : removedIndex;
            for (int i = start; i != this.indices.Count; ++i)
                --this.indices[i];

            // If the removed item didn't exist in our view, just ignore it.
            if (removedIndex < 0)
                return;

            // Remove the item.
            this.indices.RemoveAt(removedIndex);

            // Notify our listeners that the item was removed.
            this.CreateNotifier().Removed(removedIndex, item);
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            var oldItemPassesFilter = filter(oldItem);
            var newItemPassesFilter = filter(newItem);

            if (!oldItemPassesFilter)
            {
                if (!newItemPassesFilter)
                    return;

                // Act like this is an insertion.
                var newIndex = ~(this.indices as List<int>).BinarySearch(index);
                this.indices.Insert(newIndex, index);

                // Notify our listeners that the item was added.
                this.CreateNotifier().Added(newIndex, newItem);
            }
            else
            {
                if (newItemPassesFilter)
                {
                    // This is an actual replacement.
                    var replacedIndex = (this.indices as List<int>).BinarySearch(index);
                    this.CreateNotifier().Replaced(replacedIndex, oldItem, newItem);
                }
                else
                {
                    // Act like this is a removal.
                    var removedIndex = (this.indices as List<int>).BinarySearch(index);
                    this.indices.RemoveAt(removedIndex);

                    // Notify our listeners that the item was removed.
                    this.CreateNotifier().Removed(removedIndex, oldItem);
                }
            }
        }

        /// <summary>
        /// A notification that the source collection has changed significantly.
        /// </summary>
        protected override void SourceCollectionReset()
        {
            this.ResetIndices();
            this.CreateNotifier().Reset();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// A sorted list, which provides a sorted view into a source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public sealed class SortedList<T> : IndirectListBase<T>
    {
        /// <summary>
        /// The comparer used to indirectly compare source list elements.
        /// </summary>
        private readonly IComparer<int> indexComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="comparer">The source list element comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        public SortedList(IList<T> source, IComparer<T> comparer)
            : base(source, null)
        {
            this.indexComparer = this.GetComparer(comparer);
            ((List<int>)this.indices).Sort(this.indexComparer);
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            // Update our existing indexes.
            for (int i = 0; i != this.indices.Count; ++i)
            {
                if (this.indices[i] >= index)
                    ++this.indices[i];
            }

            // Find where the new item belongs in our sorted view.
            var newIndex = this.indices.View().BinarySearch(index, this.indexComparer);
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
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
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
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            // Do a removal followed by an insert.
            // Note that the other source indices do not change, so this code is simpler than an *actual* element removal and insert.
            var removedIndex = this.indices.IndexOf(index);
            this.indices.RemoveAt(removedIndex);
            var newIndex = this.indices.View().BinarySearch(index, this.indexComparer);
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
        protected override void SourceCollectionReset()
        {
            var list = (List<int>)this.indices;
            var count = this.source.Count;
            if (count < list.Count)
            {
                list.Clear();
                list.Capacity = count;
                for (int i = 0; i != count; ++i)
                    list.Add(i);
            }
            else if (count > list.Count)
            {
                list.Capacity = count;
                for (int i = list.Count; i != count; ++i)
                    list.Add(i);
            }

            list.Sort(this.indexComparer);
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return false;
        }

        /// <summary>
        /// Sets an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            throw this.NotSupported();
        }
    }
}

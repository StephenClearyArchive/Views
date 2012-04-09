using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Concatenates a list of source lists into a single list.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class ConcatList<T> : ListBase<T>, CollectionChangedListener<IList<T>>.IResponder
    {
        /// <summary>
        /// A type that forwards changes in individual source collections to its parent <see cref="ConcatList{T}"/> instance.
        /// </summary>
        private sealed class SourceChangeResponder : CollectionChangedListener<T>.IResponder
        {
            /// <summary>
            /// The parent <see cref="ConcatList{T}"/> instance.
            /// </summary>
            private readonly ConcatList<T> parent;

            /// <summary>
            /// The source collection.
            /// </summary>
            private readonly IList<T> source;

            /// <summary>
            /// Initializes a new instance of the <see cref="SourceChangeResponder"/> class.
            /// </summary>
            /// <param name="parent">The parent <see cref="ConcatList{T}"/> instance.</param>
            /// <param name="source">The source collection.</param>
            public SourceChangeResponder(ConcatList<T> parent, IList<T> source)
            {
                this.parent = parent;
                this.source = source;
            }

            void CollectionChangedListener<T>.IResponder.Added(int index, T item)
            {
                this.parent.Added(this.source, index, item);
            }

            void CollectionChangedListener<T>.IResponder.Removed(int index, T item)
            {
                this.parent.Removed(this.source, index, item);
            }

            void CollectionChangedListener<T>.IResponder.Replaced(int index, T oldItem, T newItem)
            {
                this.parent.Replaced(this.source, index, oldItem, newItem);
            }

            void CollectionChangedListener<T>.IResponder.Reset()
            {
                this.parent.Reset(this.source);
            }
        }

        /// <summary>
        /// The sequence of source lists.
        /// </summary>
        private readonly IEnumerable<IList<T>> sources;

        /// <summary>
        /// The listener for the sequence of source lists.
        /// </summary>
        private readonly CollectionChangedListener<IList<T>> listener;

        /// <summary>
        /// Listeners for each source list.
        /// </summary>
        private IEnumerable<CollectionChangedListener<T>> listeners;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcatList&lt;T&gt;"/> class with the specified source lists.
        /// </summary>
        /// <param name="sources">The source lists to concatenate.</param>
        public ConcatList(IEnumerable<IList<T>> sources)
        {
            this.sources = sources;
            this.listener = CollectionChangedListener<IList<T>>.Create(sources, this);
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, x is INotifyCollectionChanged ? new SourceChangeResponder(this, x) : null));
        }

        /// <summary>
        /// Gets a value indicating whether this list is read-only. This list is read-only if any of its source lists are read-only.
        /// </summary>
        /// <returns>true if this list is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return this.sources.Any(x => x.IsReadOnly); }
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        void CollectionChangedListener<IList<T>>.IResponder.Added(int index, IList<T> item)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, x is INotifyCollectionChanged ? new SourceChangeResponder(this, x) : null));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        void CollectionChangedListener<IList<T>>.IResponder.Removed(int index, IList<T> item)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, x is INotifyCollectionChanged ? new SourceChangeResponder(this, x) : null));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        void CollectionChangedListener<IList<T>>.IResponder.Replaced(int index, IList<T> oldItem, IList<T> newItem)
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, x is INotifyCollectionChanged ? new SourceChangeResponder(this, x) : null));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that the collection of source collections has changed.
        /// </summary>
        void CollectionChangedListener<IList<T>>.IResponder.Reset()
        {
            this.listeners = this.sources.Select(x => CollectionChangedListener<T>.Create(x, x is INotifyCollectionChanged ? new SourceChangeResponder(this, x) : null));
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has added an item.
        /// </summary>
        private void Added(IList<T> source, int index, T item)
        {
            var sourceBaseIndex = this.FindBaseIndex(source);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Added(sourceBaseIndex + index, item);
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has removed an item.
        /// </summary>
        private void Removed(IList<T> source, int index, T item)
        {
            var sourceBaseIndex = this.FindBaseIndex(source);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Removed(sourceBaseIndex + index, item);
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has replaced an item.
        /// </summary>
        private void Replaced(IList<T> source, int index, T oldItem, T newItem)
        {
            var sourceBaseIndex = this.FindBaseIndex(source);
            if (sourceBaseIndex == -1)
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Replaced(sourceBaseIndex + index, oldItem, newItem);
        }

        /// <summary>
        /// Responds to the notification that one of the source collections has changed.
        /// </summary>
        private void Reset(IList<T> source)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// Returns a value indicating whether the collection itself may be updated, e.g., <see cref="Add"/>, <see cref="Clear"/>, etc.
        /// </summary>
        /// <returns>A value indicating whether the collection itself may be updated.</returns>
        protected override bool CanUpdateCollection()
        {
            return ListHelper.CanUpdateCollection(this.sources) ?? false;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return ListHelper.CanUpdateElementValues(this.sources) ?? false;
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        protected override void DoClear()
        {
            using (this.listener.Pause())
            using (new MultiDispose(this.listeners.Select(x => x.Pause())))
            {
                foreach (var source in this.sources)
                {
                    source.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.sources.Sum(x => x.Count);
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);
            return source[sourceIndex];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);

            using (this.listener.Pause())
            using (new MultiDispose(this.listeners.Select(x => x.Pause())))
            {
                source[sourceIndex] = item;
            }
        }

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoInsert(int index, T item)
        {
            IList<T> source;
            int sourceIndex;
            this.FindNewIndex(index, out source, out sourceIndex);

            using (this.listener.Pause())
            using (new MultiDispose(this.listeners.Select(x => x.Pause())))
            {
                source.Insert(sourceIndex, item);
            }
        }

        /// <summary>
        /// Removes an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. This index is guaranteed to be valid.</param>
        protected override void DoRemoveAt(int index)
        {
            IList<T> source;
            int sourceIndex;
            this.FindExistingIndex(index, out source, out sourceIndex);

            using (this.listener.Pause())
            using (new MultiDispose(this.listeners.Select(x => x.Pause())))
            {
                source.RemoveAt(sourceIndex);
            }
        }

        /// <summary>
        /// Finds the source list and its index for accessing a specified concatenated index.
        /// </summary>
        /// <param name="concatIndex">The concatenated index.</param>
        /// <param name="source">On return, holds the source list corresponding to the concatenated index.</param>
        /// <param name="sourceIndex">On return, holds the source list index corresponding to the concatenated index.</param>
        private void FindExistingIndex(int concatIndex, out IList<T> source, out int sourceIndex)
        {
            source = null;
            sourceIndex = concatIndex;
            foreach (var sourceList in this.sources)
            {
                if (sourceIndex < sourceList.Count)
                {
                    source = sourceList;
                    return;
                }

                sourceIndex -= sourceList.Count;
            }
        }

        /// <summary>
        /// Finds the source list and its index for inserting at a specified concatenated index.
        /// </summary>
        /// <param name="concatIndex">The concatenated index at which to insert.</param>
        /// <param name="source">On return, holds the source list corresponding to the concatenated index.</param>
        /// <param name="sourceIndex">On return, holds the source list index corresponding to the concatenated index.</param>
        private void FindNewIndex(int concatIndex, out IList<T> source, out int sourceIndex)
        {
            source = null;
            sourceIndex = concatIndex;
            foreach (var sourceList in this.sources)
            {
                if (sourceIndex <= sourceList.Count)
                {
                    source = sourceList;
                    return;
                }

                sourceIndex -= sourceList.Count;
            }
        }

        /// <summary>
        /// Finds the base index for a specified source list.
        /// </summary>
        /// <param name="source">The source list to find.</param>
        /// <returns>The base index for the specified source list.</returns>
        private int FindBaseIndex(IList<T> source)
        {
            int ret = 0;
            foreach (var sourceList in this.sources)
            {
                if (sourceList == source)
                    return ret;
                ret += sourceList.Count;
            }

            return -1;
        }
    }
}

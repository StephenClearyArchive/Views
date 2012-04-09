using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// An indirect list, which provides a layer of indirection for the index values of a source list. By default, this base class does not allow clear/insert/remove. Also, the list of redirected indices is not observed for changes.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public abstract class IndirectListBase<T> : ReadOnlySourceListBase<T>
    {
        /// <summary>
        /// The redirected index values.
        /// </summary>
        protected IList<int> indices;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectListBase&lt;T&gt;"/> class for the given source list, using the given redirected index values.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="indices">The redirected index values.</param>
        public IndirectListBase(IList<T> source, IList<int> indices)
            : base(source)
        {
            this.indices = indices;
        }

        /// <summary>
        /// Creates a new list of indices matching the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <returns>A new list of indices matching the specified source list.</returns>
        protected static List<int> DefaultIndices(IList<T> source)
        {
            var list = new List<int>(source.Count);
            for (var i = 0; i != list.Count; ++i)
                list[i] = i;
            return list;
        }

        /// <summary>
        /// A notification that the source collection has added an item.
        /// </summary>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        protected override void SourceCollectionAdded(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has removed an item.
        /// </summary>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="oldItem">The item that was removed.</param>
        protected override void SourceCollectionRemoved(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that the source collection has replaced an item.
        /// </summary>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        protected override void SourceCollectionReplaced(int index, T oldItem, T newItem)
        {
            var affectedIndices = this.indices.Where(x => x == index);
            if (!affectedIndices.Any())
                return;
            if (affectedIndices.Skip(1).Any())
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Replaced(affectedIndices.First(), oldItem, newItem);
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            return this.indices.Count;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            using (this.listener.Pause())
            {
                return this.source[this.indices[index]];
            }
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            using (this.listener.Pause())
            {
                this.source[this.indices[index]] = item;
            }
        }

        /// <summary>
        /// Returns an indirect comparer which may be used to sort or compare elements in this list, based on a source list comparer.
        /// </summary>
        /// <param name="comparer">The source list comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        /// <returns>The indirect comparer.</returns>
        public IComparer<int> GetComparer(IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return new AnonymousComparer<int> { Compare = (x, y) => comparer.Compare(this[x], this[y]) };
        }
    }
}

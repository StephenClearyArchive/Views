using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// An indirect view, which provides a layer of indirection for the index values of a source view. The list of redirected indices is not observed for changes.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public abstract class IndirectViewBase<T> : SourceViewBase<T>
    {
        /// <summary>
        /// The redirected index values.
        /// </summary>
        protected List<int> indices;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectViewBase&lt;T&gt;"/> class for the given source list, using the given redirected index values.
        /// </summary>
        /// <param name="source">The source view.</param>
        /// <param name="indices">The redirected index values. This may be <c>null</c>, but the derived class constructor must initialize <see cref="indices"/> before it completes.</param>
        public IndirectViewBase(IView<T> source, List<int> indices)
            : base(source)
        {
            Contract.Requires(source != null);
            this.indices = indices;
        }

        /// <summary>
        /// Gets the current redirected index values.
        /// </summary>
        public IList<int> RedirectedIndices
        {
            get { return this.indices; }
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.indices.Count; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get { return this.source[this.indices[index]]; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.indices != null);
        }

        /// <summary>
        /// Creates a new list of indices matching the specified source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <returns>A new list of indices matching the specified source list.</returns>
        protected static List<int> DefaultIndices(IView<T> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<List<int>>() != null);
            var list = new List<int>(source.Count);
            for (var i = 0; i != list.Count; ++i)
                list[i] = i;
            return list;
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
            var affectedIndices = this.indices.Where(x => x == index);
            if (!affectedIndices.Any())
                return;
            if (affectedIndices.Skip(1).Any())
                this.CreateNotifier().Reset();
            else
                this.CreateNotifier().Replaced(affectedIndices.First(), oldItem, newItem);
        }
    }
}

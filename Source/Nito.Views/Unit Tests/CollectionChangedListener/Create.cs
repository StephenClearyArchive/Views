using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views.Util;
using System.Collections.ObjectModel;

namespace t.CollectionChangedListener
{
    internal sealed class IntResponder : ICollectionChangedResponder<int>
    {
        void ICollectionChangedResponder<int>.Added(int index, int item)
        {
            this.Added(index, item);
        }

        void ICollectionChangedResponder<int>.Removed(int index, int item)
        {
            this.Removed(index, item);
        }

        void ICollectionChangedResponder<int>.Replaced(int index, int oldItem, int newItem)
        {
            this.Replaced(index, oldItem, newItem);
        }

        void ICollectionChangedResponder<int>.Reset()
        {
            this.Reset();
        }

        public Action<int, int> Added { get; set; }
        public Action<int, int> Removed { get; set; }
        public Action<int, int, int> Replaced { get; set; }
        public Action Reset { get; set; }
    }

    [TestClass]
    public class Create
	{
        [TestMethod]
        public void ReturnsNullIfSourceDoesNotImplementINotifyCollectionChanged()
        {
            var listener = CollectionChangedListener<int>.Create(new int[0], new IntResponder());
            Assert.IsNull(listener);
        }

        [TestMethod]
        public void ReturnsInstanceifSourceImplementsINotifyCollectionChanged()
        {
            var listener = CollectionChangedListener<int>.Create(new ObservableCollection<int>(), new IntResponder());
            Assert.IsNotNull(listener);
        }
    }
}

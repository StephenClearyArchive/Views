using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views.Util;
using System.Collections.ObjectModel;
using Views.Util.Moles;

namespace t.CollectionChangedListener
{
    [TestClass]
    public class Create
	{
        [TestMethod]
        public void ReturnsNullIfSourceDoesNotImplementINotifyCollectionChanged()
        {
            var listener = CollectionChangedListener<int>.Create(new int[0], new SICollectionChangedResponder<int>());
            Assert.IsNull(listener);
        }

        [TestMethod]
        public void ReturnsInstanceifSourceImplementsINotifyCollectionChanged()
        {
            var listener = CollectionChangedListener<int>.Create(new ObservableCollection<int>(), new SICollectionChangedResponder<int>());
            Assert.IsNotNull(listener);
        }
    }
}

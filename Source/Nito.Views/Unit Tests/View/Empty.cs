using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.View
{
    [TestClass]
    public class Empty
    {
        [TestMethod]
        public void HasCountOf0()
        {
            var view = Views.View.Empty<int>();
            Assert.AreEqual(0, view.Count());
        }

        [TestMethod]
        public void HasEmptySequence()
        {
            var view = Views.View.Empty<int>();
            SequenceAssert.AreEquivalent(new int[0], view as IList<int>);
        }
    }
}

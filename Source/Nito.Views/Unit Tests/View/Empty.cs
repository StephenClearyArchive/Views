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
        public void BasicFunctionality()
        {
            var view = Views.View.Empty<int>();
            ViewAssert.BasicFunctionality(view);
        }

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
            ViewAssert.AreEquivalent(new int[0], view);
        }

        [TestMethod]
        public void ReturnValueIsSingleton()
        {
            var view0 = Views.View.Empty<int>();
            var view1 = Views.View.Empty<int>();
            Assert.AreSame(view0, view1);
        }

        [TestMethod]
        public void ReadOnly()
        {
            var view = Views.View.Empty<int>();
            ViewAssert.CollectionIsReadOnly(view);
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.View
{
    [TestClass]
    public class Repeat
    {
        [TestMethod]
        public void HasSpecifiedCount()
        {
            var view = Views.View.Repeat(3, 5);
            Assert.AreEqual(5, view.Count());
        }

        [TestMethod]
        public void IsEquivalentToEnumerableRepeat()
        {
            var view = Views.View.Repeat(3, 5);
            ViewAssert.AreEquivalent(Enumerable.Repeat(3, 5), view);
        }

        [TestMethod]
        public void ZeroCountIsEqualToEmpty()
        {
            var view = Views.View.Repeat(3, 0);
            Assert.AreSame(Views.View.Empty<int>(), view);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void NegativeCountThrowsException()
        {
            var view = Views.View.Repeat(3, -1);
        }

        [TestMethod]
        public void ReadOnly()
        {
            var view = Views.View.Repeat(3, 5);
            ViewAssert.CollectionIsReadOnly(view);
            ViewAssert.ElementsAreReadOnly(view);
        }
    }
}

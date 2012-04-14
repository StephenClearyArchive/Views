using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.View
{
    [TestClass]
    public class Range
    {
        [TestMethod]
        public void BasicFunctionality()
        {
            var view = Views.View.Range(-2, 5);
            ViewAssert.BasicFunctionality(view);
        }

        [TestMethod]
        public void HasSpecifiedCount()
        {
            var view = Views.View.Range(3, 5);
            Assert.AreEqual(5, view.Count());
        }

        [TestMethod]
        public void IsEquivalentToEnumerableRange()
        {
            var view = Views.View.Range(3, 5);
            ViewAssert.AreEquivalent(Enumerable.Range(3, 5), view);
        }

        [TestMethod]
        public void ZeroCountIsEqualToEmpty()
        {
            var view = Views.View.Range(3, 0);
            Assert.AreSame(Views.View.Empty<int>(), view);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void NegativeCountThrowsException()
        {
            var view = Views.View.Range(3, -1);
        }

        [TestMethod]
        public void ZeroStepIsEquivalentToRepeat()
        {
            var view = Views.View.Range(3, 5, 0);
            ViewAssert.AreEquivalent(Views.View.Repeat(3, 5), view);
        }

        [TestMethod]
        public void NegativeStepGoesBackwards()
        {
            var view = Views.View.Range(3, 5, -1);
            ViewAssert.AreEquivalent(new[] { 3, 2, 1, 0, -1 }, view);
        }

        [TestMethod]
        public void LargeStepSkipsValues()
        {
            var view = Views.View.Range(3, 5, 2);
            ViewAssert.AreEquivalent(new[] { 3, 5, 7, 9, 11 }, view);
        }

        [TestMethod]
        public void LargeNegativeStepSkipsValues()
        {
            var view = Views.View.Range(-1, 5, -2);
            ViewAssert.AreEquivalent(new[] { -1, -3, -5, -7, -9 }, view);
        }

        [TestMethod]
        public void ReadOnly()
        {
            var view = Views.View.Range(3, 5);
            ViewAssert.CollectionIsReadOnly(view);
            ViewAssert.ElementsAreReadOnly(view);
        }
    }
}

﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.View
{
    [TestClass]
    public class Return
    {
        [TestMethod]
        public void BasicFunctionality()
        {
            var view = Views.View.Return(3, 5, 0, 7);
            ViewAssert.BasicFunctionality(view);
        }

        [TestMethod]
        public void HasSpecifiedNumberOfItems()
        {
            var view = Views.View.Return(3, 5);
            Assert.AreEqual(2, view.Count);
        }

        [TestMethod]
        public void HasSpecifiedItems()
        {
            var view = Views.View.Return(3, 5);
            ViewAssert.AreEquivalent(new[] { 3, 5 }, view);
        }
    }
}

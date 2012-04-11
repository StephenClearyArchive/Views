using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.SourceListBase
{
    [TestClass]
    public class GenericEnumerable
    {
        [TestMethod]
        public void EnumeratesSource()
        {
            var array = new int[] { 7, 13, 17 };
            var view = array.View();
            CollectionAssert.AreEquivalent((view as IList<int>).ToArray(), array);
        }
    }
}

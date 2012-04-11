using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace t.SourceListBase
{
    [TestClass]
    public class GenericItem
    {
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void NegativeIndexThrowsException()
        {
            var view = new int[0].View() as IList<int>;
            var ret = view[-1];
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void IndexEqualToCountThrowsException()
        {
            var view = new int[1].View() as IList<int>;
            var ret = view[1];
        }

        [TestMethod]
        public void GetReadsValue()
        {
            var view = new int[] { 13 }.View() as IList<int>;
            Assert.AreEqual(13, view[0]);
        }

        [TestMethod]
        public void SetWritesValue()
        {
            var array = new int[] { 13 };
            var view = array.View() as IList<int>;
            view[0] = 17;
            Assert.AreEqual(17, array[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetOnReadOnlySourceThrowsException()
        {
            var list = new List<int> { 13 }.AsReadOnly();
            var view = list.View() as IList<int>;
            view[0] = 17;
        }
    }
}

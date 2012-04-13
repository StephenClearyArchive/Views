using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class SequenceAssert
{
    public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
    }

    public static void ForEach<T>(IEnumerable<T> result, Action<T> test)
    {
        foreach (var value in result)
            test(value);
    }
}

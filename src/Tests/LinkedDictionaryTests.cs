using System;
using System.Collections.Generic;
using ModulesFrameworkUnity.Utils;
using NUnit.Framework;

namespace MF.UnityAdapter.Tests.ModulesFrameworkUnityPackage.Runtime.ModulesFrameworkUnity.Tests
{
    public class LinkedDictionaryTests
    {
        private static IEnumerable<TestCaseData> SortCases()
        {
            yield return new TestCaseData(new[] { 3, 5, 4, 0, 2, 1 })
                .SetName("Sort_RightPartRegressionCase");
            
            yield return new TestCaseData(new[] { 4, 3, 2, 0, 1 })
                .SetName("Sort_PreviousRegressionCase");

            yield return new TestCaseData(new[] { 5, 4, 3, 2, 1, 0 })
                .SetName("Sort_Descending");

            yield return new TestCaseData(new[] { 0, 1, 2, 3, 4, 5 })
                .SetName("Sort_AlreadySorted");

            yield return new TestCaseData(new[] { 2, 1, 2, 0, 1, 0 })
                .SetName("Sort_Duplicates");
        }
        
        [TestCaseSource(nameof(SortCases))]
        public void Collection_Sort_ProducesExactExpectedOrder(int[] values)
        {
            var linkedDictionary = new LinkedDictionary<int, int>();

            for (var i = 0; i < values.Length; i++)
                linkedDictionary.Add(i, values[i]);

            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));

            var expected = (int[])values.Clone();
            Array.Sort(expected);

            CollectionAssert.AreEqual(expected, linkedDictionary.Values);
            Assert.That(linkedDictionary.Count, Is.EqualTo(values.Length));
        }

        [Test]
        public void Collection_Sort_KeepsDictionaryNodesValid()
        {
            var values = new[] { 30, 10, 20, 10 };
            var linkedDictionary = new LinkedDictionary<int, int>();

            for (var i = 0; i < values.Length; i++)
                linkedDictionary.Add(i, values[i]);

            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));

            for (var i = 0; i < values.Length; i++)
                Assert.That(linkedDictionary[i].Value, Is.EqualTo(values[i]));

            Assert.That(linkedDictionary.FirstNode.Previous, Is.Null);
            Assert.That(linkedDictionary.LastNode.Next, Is.Null);
            CollectionAssert.AreEqual(new[] { 10, 10, 20, 30 }, linkedDictionary.Values);
        }

        [Test]
        public void Collection_Sort_WithDescendingComparer_SortsDescending()
        {
            var values = new[] { 3, 1, 4, 1, 5, 9, 2 };
            var linkedDictionary = new LinkedDictionary<int, int>();

            for (var i = 0; i < values.Length; i++)
                linkedDictionary.Add(i, values[i]);

            linkedDictionary.Sort(static (n1, n2) => n2.CompareTo(n1));

            CollectionAssert.AreEqual(new[] { 9, 5, 4, 3, 2, 1, 1 }, linkedDictionary.Values);
        }
        
        [Test]
        public void CollectionSorted_Sort_DoesNotAffect()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();
            linkedDictionary.Add(0, 0);
            linkedDictionary.Add(1, 1);
            linkedDictionary.Add(2, 2);
            linkedDictionary.Add(3, 3);

            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));
            
            var current = linkedDictionary.FirstNode;
            while (current != null)
            {
                Assert.LessOrEqual(current.Value, current.Next?.Value ?? int.MaxValue);
                current = current.Next;
            }
        }
        
        [Test]
        public void CollectionSortedDescending_Sort_CorrectSorting()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();
            linkedDictionary.Add(3, 3);
            linkedDictionary.Add(2, 2);
            linkedDictionary.Add(1, 1);
            linkedDictionary.Add(0, 0);

            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));
            
            var current = linkedDictionary.FirstNode;
            while (current != null)
            {
                Assert.LessOrEqual(current.Value, current.Next?.Value ?? int.MaxValue);
                current = current.Next;
            }
        }
        
        [Test]
        public void CollectionEmpty_Sort_NoErrors()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();

            Assert.DoesNotThrow(() => linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2)));
        }
        
        [Test]
        public void CollectionOneElement_Sort_NoErrors()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();
            linkedDictionary.Add(3, 3);

            Assert.DoesNotThrow(() => linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2)));
        }
        
        [Test]
        public void CollectionTwoElementsUnsorted_Sort_CorrectSorting()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();
            linkedDictionary.Add(3, 3);
            linkedDictionary.Add(1, 1);

            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));
            
            var current = linkedDictionary.FirstNode;
            while (current != null)
            {
                Assert.LessOrEqual(current.Value, current.Next?.Value ?? int.MaxValue);
                current = current.Next;
            }
        }
        
        [Test]
        public void CollectionOf4SameElement_Sort_NotAffects()
        {
            var linkedDictionary = new LinkedDictionary<string, int>();
            linkedDictionary.Add("1a", 1);
            linkedDictionary.Add("1b", 1);
            linkedDictionary.Add("1c", 1);
            linkedDictionary.Add("1d", 1);

            var oldFirst = linkedDictionary.FirstNode;
            var oldLast = linkedDictionary.LastNode;
            linkedDictionary.Sort(static (n1, n2) => n1.CompareTo(n2));
            
            Assert.That(oldFirst, Is.EqualTo(linkedDictionary.FirstNode));
            Assert.That(oldLast, Is.EqualTo(linkedDictionary.LastNode));
        }
    }
}

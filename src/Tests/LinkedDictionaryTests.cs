using ModulesFrameworkUnity.Utils;
using NUnit.Framework;

namespace MF.UnityAdapter.Tests.ModulesFrameworkUnityPackage.Runtime.ModulesFrameworkUnity.Tests
{
    public class LinkedDictionaryTests
    {
        [Test]
        public void CollectionUnSorted_Sort_CorrectSorting()
        {
            var linkedDictionary = new LinkedDictionary<int, int>();
            linkedDictionary.Add(1, 1);
            linkedDictionary.Add(3, 3);
            linkedDictionary.Add(2, 2);
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

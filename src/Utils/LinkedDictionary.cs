using System;
using System.Collections;
using System.Collections.Generic;

namespace ModulesFrameworkUnity.Utils
{
    internal class LinkedDictionary<TKey, T> : IEnumerable<T>
    {
        private readonly Dictionary<TKey, LinkedListNode<T>> _dictionary = new();
        private readonly LinkedList<T> _linkedList = new();

        public IEnumerable<T> Values => _linkedList;

        public LinkedListNode<T> this[TKey key] => _dictionary[key];

        public LinkedListNode<T> FirstNode => _linkedList.First;
        public LinkedListNode<T> LastNode => _linkedList.Last;
        public int Count => _linkedList.Count;

        public void Add(TKey key, T value)
        {
            var node = new LinkedListNode<T>(value);
            _dictionary.Add(key, node);
            _linkedList.AddLast(node);
        }

        public void Remove(TKey key)
        {
            if (!_dictionary.TryGetValue(key, out var node))
                return;

            _linkedList.Remove(node);
            _dictionary.Remove(key);
        }

        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Sort(Func<T, T, int> comparer)
        {
            if (_linkedList.Count < 2)
                return;

            MergeSort(_linkedList.First, _linkedList.Last, comparer);
        }

        private (LinkedListNode<T> newLeft, LinkedListNode<T> newRight) MergeSort(
            LinkedListNode<T> left, 
            LinkedListNode<T> right,
            Func<T, T, int> comparer)
        {
            // one element subset
            if (left == right)
                return (left, right);

            // 2-elements subset
            if (left.Next == right)
            {
                if (comparer(left.Value, right.Value) <= 0)
                    return (left, right);

                _linkedList.Remove(left);
                _linkedList.AddAfter(right, left);
                return (right, left);
            }

            var pivot = ChoosePivot(left, right);

            var (firstLeft, _) = MergeSort(left, pivot.Previous, comparer);
            var (secondLeft, secondRight) = MergeSort(pivot, right, comparer);

            return Merge(firstLeft, secondLeft, secondRight, comparer);
        }

        private (LinkedListNode<T> newLeft, LinkedListNode<T> newRight) Merge(
            LinkedListNode<T> firstLeft,
            LinkedListNode<T> secondLeft,
            LinkedListNode<T> secondRight,
            Func<T, T, int> comparer)
        {
            var head = firstLeft;
            var head2 = secondLeft;
            var resultHead = firstLeft;
            var resultTail = secondRight;
            var head2Next = secondRight.Next;
            
            while (head2 != head2Next && head != head2)
            {
                var comparisonResult = comparer(head2.Value, head.Value);
                if (comparisonResult < 0)
                {
                    var newHead2 = head2.Next;
                    if (head2 == secondRight)
                        resultTail = secondRight.Previous;

                    _linkedList.Remove(head2);
                    _linkedList.AddBefore(head, head2);
                    if (comparer(head2.Value, resultHead.Value) < 0)
                        resultHead = head2;
                    head2 = newHead2;
                }
                else
                {
                    head = head.Next;
                }
            }

            return (resultHead, resultTail);
        }
        
        private LinkedListNode<T> ChoosePivot(LinkedListNode<T> left, LinkedListNode<T> right)
        {
            if (left == right || left.Next == right)
                return left;

            var size = LengthBetween(left, right);
            var newSize = size / 2;
            if (newSize == 1) // size == 3
                return left.Next;

            var current = left;
            while (newSize > 0 && current.Next != null)
            {
                newSize--;
                current = current.Next;
            }
            return current;
        }

        /// <summary>
        ///     Returns size of LinkedList between left and right nodes including both
        /// </summary>
        private static int LengthBetween(LinkedListNode<T> left, LinkedListNode<T> right)
        {
            if (left == right)
                return 1;

            if (left == null || right == null)
                return 0;

            var current = left;
            var size = 2;
            while (current.Next != right && current.Next != null)
            {
                size++;
                current = current.Next;
            }

            return size;
        }
    }
}

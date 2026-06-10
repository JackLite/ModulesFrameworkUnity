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

            QSort(_linkedList.First, _linkedList.Last, comparer);
        }

        private void QSort(LinkedListNode<T> left, LinkedListNode<T> right, Func<T, T, int> comparer)
        {
            if (left == right)
                return;

            if (left.Next == right)
            {
                if (comparer(left.Value, right.Value) <= 0)
                    return;

                _linkedList.Remove(left);
                _linkedList.AddAfter(right, left);
                return;
            }

            // choose pivot
            var pivot = ChoosePivot(left, right);
            // ensure that left <= pivot && right >= pivot
            while (comparer(left.Value, pivot.Value) > 0 && left != pivot)
            {
                var next = left.Next;
                _linkedList.Remove(left);
                _linkedList.AddAfter(pivot, left);
                left = next;
            }

            while (comparer(right.Value, pivot.Value) < 0 && right != pivot)
            {
                var prev = right.Previous;
                _linkedList.Remove(right);
                _linkedList.AddBefore(pivot, right);
                right = prev;
            }

            SortLeftPart(pivot, left, comparer);
            SortRightPart(pivot, right, comparer);

            QSort(left, pivot, comparer);
            QSort(pivot, right, comparer);
        }

        private void SortLeftPart(LinkedListNode<T> pivot, LinkedListNode<T> left, Func<T, T, int> comparer)
        {
            if (left == pivot)
                return;

            var current = left.Next;
            var infinitySafe = 100;
            while (current != null && current != pivot && infinitySafe-- > 0)
            {
                var next = current.Next;
                if (comparer(current.Value, pivot.Value) > 0)
                {
                    _linkedList.Remove(current);
                    _linkedList.AddAfter(pivot, current);
                }
                current = next;
            }
        }

        private void SortRightPart(LinkedListNode<T> pivot, LinkedListNode<T> right, Func<T, T, int> comparer)
        {
            if (right == pivot)
                return;
            var current = right.Previous;
            var infinitySafe = 100;
            while (current != null && current != pivot && infinitySafe-- > 0)
            {
                var prev = current.Previous;
                if (comparer(current.Value, pivot.Value) < 0)
                {
                    _linkedList.Remove(current);
                    _linkedList.AddBefore(pivot, current);
                }
                current = prev;
            }
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

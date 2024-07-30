using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModulesFrameworkUnity.Utils
{
    internal class LinkedDictionary<TKey, T> : IEnumerable<KeyValuePair<TKey, T>>
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

        public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        {
            return _dictionary.Select(kvp => new KeyValuePair<TKey, T>(kvp.Key, kvp.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
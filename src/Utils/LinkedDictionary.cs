using System.Collections.Generic;

namespace ModulesFrameworkUnity.Utils
{
    internal class LinkedDictionary<TKey, T>
    {
        private readonly Dictionary<TKey, LinkedListNode<T>> _dictionary = new();
        private readonly LinkedList<T> _linkedList = new();

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
    }
}
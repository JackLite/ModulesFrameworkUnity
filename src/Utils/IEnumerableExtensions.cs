using System.Collections.Generic;

namespace ModulesFrameworkUnity.Utils
{
    internal static class IEnumerableExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
        {
            var queue = new Queue<T>();
            foreach (var item in enumerable)
                queue.Enqueue(item);
            return queue;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Utility
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Flatten tree via LINQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getChildren"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> getChildren)
        {
            var stack = new Stack<T>();
            foreach (var item in items)
            {
                stack.Push(item);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;

                var children = getChildren(current);
                if (children == null)
                {
                    continue;
                }

                foreach (var child in children)
                {
                    stack.Push(child);
                }
            }
        }

    }
}

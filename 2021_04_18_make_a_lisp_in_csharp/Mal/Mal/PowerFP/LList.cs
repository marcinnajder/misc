using System;
using System.Collections.Generic;

namespace PowerFP
{
    public record LList<T>(T Head, LList<T>? Tail) { }

    public static class LListM
    {
        public static IEnumerable<T> ToEnumerable<T>(this LList<T>? llist)
        {
            if (llist == null)
            {
                yield break;
            }

            var node = llist;
            while (node != null)
            {
                yield return node.Head;
                node = node.Tail;
            }
        }

        public static IEnumerable<T> ToEnumerableRec<T>(this LList<T>? llist)
        {
            if (llist == null)
            {
                yield break;
            }

            yield return llist.Head;

            foreach (var item in ToEnumerableRec(llist.Tail))
            {
                yield return item;
            }
        }

        public static LList<T>? ToLList<T>(this IEnumerable<T> llist)
        {
            return NextValue(llist.GetEnumerator());
            static LList<T>? NextValue(IEnumerator<T> e) => e.MoveNext() ? new LList<T>(e.Current, NextValue(e)) : null;
        }


        public static LList<T>? Reverse<T>(this LList<T>? llist)
        {
            return llist switch
            {
                null => null,
                LList<T>(var Head, var Tails) => null,
                // _ => null
            };
        }

        // todo, Map, Filter, Reduce
    }
}


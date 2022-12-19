using System.Collections.Generic;

namespace Framework
{
    public static class DictionaryExtentions
    {
        public static TValue GetOrAddNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) 
            where TValue : class, new()
        {
            TValue val;

            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }
    }

    public static class ListExtentions
    {
        public static bool TryPopLast<T>(this List<T> list, out T item)
        {
            int lastIdx = list.Count - 1;
            if (lastIdx >= 0)
            {
                item = list[lastIdx];
                list.RemoveAt(lastIdx);
                return true;
            }

            item = default;
            return false;
        }

        public static void PopLast<T>(this List<T> list)
        {
            int lastIdx = list.Count - 1;
            if (lastIdx >= 0)
                list.RemoveAt(lastIdx);
        }

        public static void SwapRemoveAt<T>(this List<T> list, int idx)
        {
            int lastIdx = list.Count - 1;
            if (lastIdx >= 0)
            {
                list[lastIdx] = list[idx];
                list.RemoveAt(lastIdx);
            }
        }

        public static void SwapRemove<T>(this List<T> list, T val)
        {
            int idx = list.IndexOf(val);
            if (idx > 0)
                SwapRemoveAt(list, idx);
        }

    }
}

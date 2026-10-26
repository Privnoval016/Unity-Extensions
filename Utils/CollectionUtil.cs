using System;
using System.Collections.Generic;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utility extensions for collections like dictionaries and lists.
     * Includes inverse lookup operations, a reference membership check and a circular list wrapper.
     * </summary>
     */
    public static class CollectionUtil
    {
        /**
         * <summary>
         * Performs an inverse lookup in a dictionary, returning the first key that maps to the specified value.
         * </summary>
         * <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
         * <typeparam name="TValue">The type of values in the dictionary.</typeparam>
         * <param name="dict">The dictionary to search.</param>
         * <param name="value">The value to search for.</param>
         * <returns>The first key that maps to the specified value.</returns>
         * <exception cref="KeyNotFoundException">Thrown when the specified value is not found in the dictionary.</exception>
         */
        public static TKey InverseLookup<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
        {
            foreach (var kvp in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    return kvp.Key;
                }
            }

            throw new KeyNotFoundException("The specified value was not found in the dictionary.");
        }

        /**
         * <summary>
         * Attempts to perform an inverse lookup in a dictionary.
         * Returns the first key that maps to the specified value if found.
         * </summary>
         * <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
         * <typeparam name="TValue">The type of values in the dictionary.</typeparam>
         * <param name="dict">The dictionary to search.</param>
         * <param name="value">The value to search for.</param>
         * <param name="key">When this method returns, contains the first key that maps to the specified value if found; otherwise, the default value.</param>
         * <returns>true if the value was found; otherwise, false.</returns>
         */
        public static bool TryInverseLookup<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value, out TKey key)
        {
            foreach (var kvp in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    key = kvp.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }

        /**
         * <summary>
         * Whether a list holds this exact object (reference equality: an overridden <c>Equals</c> or <c>==</c> is
         * never consulted, so a destroyed Unity object is not taken for null). Walks by index, so nothing is allocated.
         * </summary>
         * <typeparam name="T">The type of elements in the list.</typeparam>
         * <param name="items">The list to search; null holds nothing.</param>
         * <param name="item">The object to look for; null finds a null entry.</param>
         * <returns>true if the list holds the object; otherwise, false.</returns>
         */
        public static bool Has<T>(this IReadOnlyList<T> items, T item) where T : class
        {
            if (items == null) return false;
            for (var i = 0; i < items.Count; i++)
            {
                if (ReferenceEquals(items[i], item)) return true;
            }

            return false;
        }
    }

    /**
     * <summary>
     * A circular list that wraps around when accessed with indices beyond its bounds.
     * Useful for cyclic data structures that need to loop back to the beginning.
     * </summary>
     * <typeparam name="T">The type of elements in the list.</typeparam>
     */
    [Serializable]
    public class CircularList<T> : List<T>
    {
        /**
         * <summary>
         * Initializes a new instance of the <see cref="CircularList{T}"/> class.
         * </summary>
         */
        public CircularList()
        {
        }

        /**
         * <summary>
         * Initializes a new instance of the <see cref="CircularList{T}"/> class from an existing list.
         * </summary>
         * <param name="list">The list to copy items from.</param>
         */
        public CircularList(List<T> list) : base(list)
        {
        }

        /**
         * <summary>
         * Returns the item at a shifted index, wrapping around the list if the index exceeds bounds.
         * </summary>
         * <param name="current">The current index.</param>
         * <param name="shift">The amount to shift by (can be negative).</param>
         * <returns>The item at the calculated shifted index.</returns>
         */
        public T ItemAtShiftedIndex(int current, int shift)
        {
            return this[ShiftedIndex(current, shift)];
        }

        /**
         * <summary>
         * Calculates a shifted index with wrapping, handling negative indices correctly.
         * </summary>
         * <param name="current">The current index.</param>
         * <param name="shift">The amount to shift by (can be negative).</param>
         * <returns>The shifted index, wrapped to the valid range [0, Count).</returns>
         */
        public int ShiftedIndex(int current, int shift)
        {
            int index = (current + shift) % Count;
            if (index < 0)
            {
                index = Count + index;
            }

            return index;
        }
    }
}



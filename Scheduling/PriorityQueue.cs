using System;
using System.Collections.Generic;

namespace Extensions.Scheduling
{
    /**
     * <summary>
     * Generic stable indexed priority queue backed by a binary min-heap. Supports O(log n) enqueue,
     * dequeue-minimum, in-place priority updates, and removal by item. Entries with equal priority
     * always dequeue in the order they were enqueued.
     * </summary>
     * <typeparam name="TItem">The type of item stored in the queue.</typeparam>
     */
    public class PriorityQueue<TItem>
    {
        private struct Entry
        {
            public TItem Item;
            public float Priority;
            public long Sequence;
        }

        #region Private Fields

        private readonly List<Entry> _heap = new();
        private readonly Dictionary<TItem, int> _indexByItem;
        private long _nextSequence;

        #endregion

        #region Properties

        /**
         * <summary>The number of items currently in the queue.</summary>
         */
        public int Count => _heap.Count;

        #endregion

        /**
         * <summary>
         * Creates an empty priority queue.
         * </summary>
         * <param name="comparer">
         * Optional equality comparer used to identify items for <see cref="ChangePriority" /> and
         * <see cref="Remove" />. Defaults to <see cref="EqualityComparer{TItem}.Default" />.
         * </param>
         */
        public PriorityQueue(IEqualityComparer<TItem> comparer = null)
        {
            _indexByItem = new Dictionary<TItem, int>(comparer ?? EqualityComparer<TItem>.Default);
        }

        #region Public API

        /** <summary>An independent queue holding the same items at the same priorities, ties still broken by when each was first enqueued here.</summary> */
        public PriorityQueue<TItem> Clone()
        {
            var clone = new PriorityQueue<TItem>(_indexByItem.Comparer) { _nextSequence = _nextSequence };
            clone._heap.AddRange(_heap);
            foreach (var pair in _indexByItem) clone._indexByItem[pair.Key] = pair.Value;
            return clone;
        }

        /**
         * <summary>Adds an item to the queue at the given priority. Lower priority dequeues first.</summary>
         * <param name="item">The item to enqueue. Must not already be present in the queue.</param>
         * <param name="priority">The item's priority; lower values are dequeued first.</param>
         */
        public void Enqueue(TItem item, float priority)
        {
            if (_indexByItem.ContainsKey(item))
            {
                throw new ArgumentException($"Item {item} is already present in the queue.", nameof(item));
            }

            var entry = new Entry { Item = item, Priority = priority, Sequence = _nextSequence++ };
            _heap.Add(entry);

            var index = _heap.Count - 1;
            _indexByItem[item] = index;
            SiftUp(index);
        }

        /**
         * <summary>Returns the item with the lowest priority without removing it.</summary>
         */
        public TItem Peek()
        {
            if (!TryPeek(out var item, out _))
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return item;
        }

        /**
         * <summary>Attempts to read the item with the lowest priority without removing it.</summary>
         * <param name="item">The lowest-priority item, or default if the queue is empty.</param>
         * <param name="priority">That item's priority, or default if the queue is empty.</param>
         * <returns>True if the queue was non-empty.</returns>
         */
        public bool TryPeek(out TItem item, out float priority)
        {
            if (_heap.Count == 0)
            {
                item = default;
                priority = default;
                return false;
            }

            item = _heap[0].Item;
            priority = _heap[0].Priority;
            return true;
        }

        /**
         * <summary>Removes and returns the item with the lowest priority.</summary>
         */
        public TItem Dequeue()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            var root = _heap[0].Item;
            RemoveAt(0);
            return root;
        }

        /**
         * <summary>
         * Updates an already-queued item's priority in place, re-ordering the heap as needed.
         * </summary>
         * <param name="item">The item to update. Must already be present in the queue.</param>
         * <param name="newPriority">The item's new priority.</param>
         */
        public void ChangePriority(TItem item, float newPriority)
        {
            var index = IndexOf(item);
            var oldPriority = _heap[index].Priority;

            var entry = _heap[index];
            entry.Priority = newPriority;
            _heap[index] = entry;

            if (newPriority < oldPriority)
            {
                SiftUp(index);
            }
            else
            {
                SiftDown(index);
            }
        }

        /**
         * <summary>Reads an already-queued item's current priority without dequeuing or modifying it.</summary>
         * <returns>True if the item is present in the queue.</returns>
         */
        public bool TryGetPriority(TItem item, out float priority)
        {
            if (!_indexByItem.TryGetValue(item, out var index))
            {
                priority = default;
                return false;
            }

            priority = _heap[index].Priority;
            return true;
        }

        /**
         * <summary>Removes an item from the queue if present.</summary>
         * <returns>True if the item was found and removed.</returns>
         */
        public bool Remove(TItem item)
        {
            if (!_indexByItem.TryGetValue(item, out var index))
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        /**
         * <summary>Returns true if the given item is currently queued.</summary>
         */
        public bool Contains(TItem item) => _indexByItem.ContainsKey(item);

        /**
         * <summary>
         * Returns up to <paramref name="count" /> queued items in dequeue order without removing them.
         * Useful for previewing upcoming turn order in a UI.
         * </summary>
         */
        public IReadOnlyList<TItem> PeekOrdered(int count)
        {
            var ordered = new List<Entry>(_heap);
            ordered.Sort(CompareEntries);

            var take = Math.Min(count, ordered.Count);
            var result = new List<TItem>(take);
            for (var i = 0; i < take; i++)
            {
                result.Add(ordered[i].Item);
            }

            return result;
        }

        #endregion

        #region Helpers

        private static int CompareEntries(Entry a, Entry b)
        {
            var priorityComparison = a.Priority.CompareTo(b.Priority);
            return priorityComparison != 0 ? priorityComparison : a.Sequence.CompareTo(b.Sequence);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                var parent = (index - 1) / 2;
                if (CompareEntries(_heap[index], _heap[parent]) >= 0)
                {
                    break;
                }

                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                var left = index * 2 + 1;
                var right = index * 2 + 2;
                var smallest = index;

                if (left < _heap.Count && CompareEntries(_heap[left], _heap[smallest]) < 0)
                {
                    smallest = left;
                }

                if (right < _heap.Count && CompareEntries(_heap[right], _heap[smallest]) < 0)
                {
                    smallest = right;
                }

                if (smallest == index)
                {
                    break;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void RemoveAt(int index)
        {
            var item = _heap[index].Item;
            var lastIndex = _heap.Count - 1;

            Swap(index, lastIndex);
            _heap.RemoveAt(lastIndex);
            _indexByItem.Remove(item);

            if (index < _heap.Count)
            {
                SiftDown(index);
                SiftUp(index);
            }
        }

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
            _indexByItem[_heap[i].Item] = i;
            _indexByItem[_heap[j].Item] = j;
        }

        private int IndexOf(TItem item)
        {
            if (!_indexByItem.TryGetValue(item, out var index))
            {
                throw new KeyNotFoundException($"Item {item} is not present in the queue.");
            }

            return index;
        }

        #endregion
    }
}

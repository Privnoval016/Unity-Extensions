using System;
using System.Collections.Generic;

namespace Extensions.Modifiers
{
    /**
     * <summary>
     * Numeric-generic sibling of <see cref="Mediator{T}" />. Manages queries and modifiers over a
     * <typeparamref name="TValue" /> value instead of a hardcoded <see langword="int" />. Orders
     * modifiers by <c>(Priority, RegistrationSequence)</c> explicitly, so equal-priority modifiers
     * always resolve in registration order regardless of sort-implementation details.
     * </summary>
     */
    public class Mediator<TQueryKey, TValue> where TQueryKey : IQueryKey<TQueryKey>
    {
        private readonly List<Modifier<TQueryKey, TValue>> modifiers = new();

        /** <summary>The registered modifiers in resolution order, for display; register and remove through the methods.</summary> */
        public IReadOnlyList<Modifier<TQueryKey, TValue>> Modifiers => modifiers;
        private long nextRegistrationSequence;

        public Action OnModifiersChanged = delegate { };

        public void PerformQuery(object sender, QueryContext<TQueryKey, TValue> queryContext)
        {
            foreach (var modifier in modifiers)
            {
                modifier.Handle(sender, queryContext);
            }
        }

        public void AddModifier(Modifier<TQueryKey, TValue> mod)
        {
            mod.RegistrationSequence = nextRegistrationSequence++;

            modifiers.Add(mod);
            modifiers.Sort(CompareModifiers);

            mod.MarkedForRemoval = false;

            mod.OnDisposed += _ =>
            {
                // By reference: Modifier equality is key + strategy type, so Remove would take the first
                // look-alike (e.g. a permanent +10 when a +5 buff expires).
                modifiers.RemoveAll(m => ReferenceEquals(m, mod));
                OnModifiersChanged?.Invoke();
            };

            OnModifiersChanged?.Invoke();
        }

        public void Update()
        {
            foreach (var mod in modifiers)
            {
                mod.Update();
            }

            for (var i = modifiers.Count - 1; i >= 0; i--)
            {
                if (modifiers[i].MarkedForRemoval)
                {
                    modifiers[i].Dispose();
                }
            }
        }

        private static int CompareModifiers(Modifier<TQueryKey, TValue> a, Modifier<TQueryKey, TValue> b)
        {
            var priorityComparison = a.Priority.CompareTo(b.Priority);
            return priorityComparison != 0 ? priorityComparison : a.RegistrationSequence.CompareTo(b.RegistrationSequence);
        }
    }
}

using System;

namespace Extensions.Modifiers
{
    /**
     * <summary>
     * Numeric-generic sibling of <see cref="Modifier{T}" />. Represents a modifier that can alter a
     * <typeparamref name="TValue" /> value based on a specific strategy and can be applied for a
     * certain duration.
     * </summary>
     */
    public class Modifier<TQueryKey, TValue> where TQueryKey : IQueryKey<TQueryKey>
    {
        public TQueryKey Key { get; }
        public IModifierStrategy<TValue> Strategy { get; }

        public int Priority { get; set; } = 0;

        /**
         * <summary>
         * The order in which this modifier was registered with its <see cref="Mediator{TQueryKey,TValue}" />.
         * Used as an explicit tiebreak so equal-<see cref="Priority" /> modifiers still resolve in a
         * deterministic order, rather than relying on <see cref="System.Collections.Generic.List{T}.Sort" />'s
         * incidental (and unguaranteed) stability.
         * </summary>
         */
        public long RegistrationSequence { get; internal set; }

        public bool MarkedForRemoval { get; set; }

        /** <summary>How long this modifier lasts; null for permanent.</summary> */
        public IModifierDuration Duration => duration;

        public event Action<Modifier<TQueryKey, TValue>> OnDisposed = delegate { };

        private readonly IModifierDuration duration;

        public Modifier(TQueryKey key, IModifierStrategy<TValue> strategy, float duration = 0f, int priority = 0)
        {
            Key = key;
            Strategy = strategy;
            Priority = priority;

            if (duration <= 0f) return;

            this.duration = new TimerModifierDuration(duration);
        }

        public Modifier(TQueryKey key, IModifierStrategy<TValue> strategy, IModifierDuration duration, int priority = 0)
        {
            Key = key;
            Strategy = strategy;
            Priority = priority;
            this.duration = duration;
        }

        public void Update()
        {
            duration?.Tick();

            if (duration is { IsFinished: true })
            {
                MarkedForRemoval = true;
            }
        }

        public void Handle(object sender, QueryContext<TQueryKey, TValue> queryContext)
        {
            if (!Key.Equals(queryContext.Key)) return;

            (queryContext.BaseValue, queryContext.CurrentValue) =
                Strategy.Modify(queryContext.BaseValue, queryContext.CurrentValue);
        }

        public void Dispose()
        {
            MarkedForRemoval = true;
            OnDisposed?.Invoke(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is Modifier<TQueryKey, TValue> other)
            {
                return Key.Equals(other.Key) && Strategy.Equals(other.Strategy);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Strategy);
        }

        public static bool operator ==(Modifier<TQueryKey, TValue> left, Modifier<TQueryKey, TValue> right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Modifier<TQueryKey, TValue> left, Modifier<TQueryKey, TValue> right)
        {
            return !(left == right);
        }
    }
}

namespace Extensions.Modifiers
{
    /**
     * <summary>
     * Numeric-generic sibling of <see cref="QueryContext{TQueryKey}" />. Carries a
     * <typeparamref name="TValue" /> base/current value pair instead of a hardcoded
     * <see langword="int" />, for queries (e.g. stats stacked with many percentage modifiers) where
     * integer rounding across the stack is undesirable.
     * </summary>
     */
    public class QueryContext<TQueryKey, TValue> where TQueryKey : IQueryKey<TQueryKey>
    {
        public TQueryKey Key;
        public TValue BaseValue;
        public TValue CurrentValue;

        public QueryContext(TQueryKey key, TValue baseValue, TValue currentValue)
        {
            Key = key;
            BaseValue = baseValue;
            CurrentValue = currentValue;
        }
    }
}

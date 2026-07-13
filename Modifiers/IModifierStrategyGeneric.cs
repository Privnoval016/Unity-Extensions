namespace Extensions.Modifiers
{
    /**
     * <summary>
     * Numeric-generic sibling of <see cref="IModifierStrategy" />. Strategy for modifying a stat query
     * result of type <typeparamref name="TValue" /> instead of a hardcoded <see langword="int" />.
     * </summary>
     * <typeparam name="TValue">The numeric value type this strategy operates on (e.g. <see langword="float" />).</typeparam>
     */
    public abstract class IModifierStrategy<TValue>
    {
        public abstract (TValue, TValue) Modify(TValue baseValue, TValue currentValue);

        public override bool Equals(object obj)
        {
            return obj?.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public static bool operator ==(IModifierStrategy<TValue> left, IModifierStrategy<TValue> right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(IModifierStrategy<TValue> left, IModifierStrategy<TValue> right)
        {
            return !(left == right);
        }
    }
}

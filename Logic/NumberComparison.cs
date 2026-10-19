namespace Extensions.Logic
{
    /** <summary>How a value compares to a number, for conditions on counts, levels and flags.</summary> */
    public enum NumberComparison
    {
        /** <summary>At least the number.</summary> */
        AtLeast,
        /** <summary>Exactly the number.</summary> */
        Equal,
        /** <summary>Below the number.</summary> */
        Below,
    }

    /** <summary>Applies a <see cref="NumberComparison" />.</summary> */
    public static class NumberComparisons
    {
        /** <summary>True when the value compares to the number as asked.</summary> */
        public static bool Holds(NumberComparison comparison, int value, int number) => comparison switch
        {
            NumberComparison.Equal => value == number,
            NumberComparison.Below => value < number,
            _ => value >= number,
        };
    }
}

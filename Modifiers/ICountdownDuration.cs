namespace Extensions.Modifiers
{
    /** <summary>Optional capability of an <see cref="IModifierDuration" /> that counts whole steps (turns): how many are left, for display.</summary> */
    public interface ICountdownDuration
    {
        int Remaining { get; }
    }
}

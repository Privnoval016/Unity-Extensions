namespace Extensions.Modifiers
{
    /**
     * <summary>
     * A pluggable duration source for a <see cref="Modifier{T}" />. Decouples "when does this modifier
     * expire" from any particular clock, so a modifier can be driven by real time
     * (<see cref="TimerModifierDuration" />) or by explicit, caller-controlled ticks
     * (<see cref="ManualCountdownDuration" />) without the modifier itself knowing the difference.
     * </summary>
     */
    public interface IModifierDuration
    {
        /**
         * <summary>True once this duration has elapsed and the owning modifier should be removed.</summary>
         */
        bool IsFinished { get; }

        /**
         * <summary>Advances this duration by one unit. What a "unit" means depends on the implementation.</summary>
         */
        void Tick();
    }
}

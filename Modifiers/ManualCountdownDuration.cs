namespace Extensions.Modifiers
{
    /**
     * <summary>
     * An <see cref="IModifierDuration" /> that only advances when <see cref="Tick" /> is called
     * explicitly by owning code, with no dependency on real time or any timer/update loop. Suited to
     * turn-counted or step-counted modifiers (a buff lasting "3 turns" rather than "3 seconds") in a
     * headless or otherwise non-real-time-driven simulation.
     * </summary>
     */
    public class ManualCountdownDuration : IModifierDuration, ICountdownDuration
    {
        private int _remainingTicks;

        /**
         * <summary>Creates a duration that expires after <paramref name="totalTicks" /> calls to <see cref="Tick" />.</summary>
         */
        public ManualCountdownDuration(int totalTicks)
        {
            _remainingTicks = totalTicks;
        }

        public bool IsFinished => _remainingTicks <= 0;

        public int Remaining => _remainingTicks;

        public void Tick()
        {
            if (_remainingTicks > 0)
            {
                _remainingTicks--;
            }
        }
    }
}

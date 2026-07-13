using Extensions.Timers;

namespace Extensions.Modifiers
{
    /**
     * <summary>
     * An <see cref="IModifierDuration" /> backed by a real-time <see cref="CountdownTimer" />. This is
     * the duration source <see cref="Modifier{T}" />'s float-duration constructor has always used.
     * </summary>
     */
    public class TimerModifierDuration : IModifierDuration
    {
        private readonly CountdownTimer _timer;

        /**
         * <summary>Creates a duration that expires after <paramref name="seconds" /> of real time.</summary>
         */
        public TimerModifierDuration(float seconds)
        {
            _timer = new CountdownTimer(seconds);
            _timer.Start();
        }

        public bool IsFinished => _timer.IsFinished;

        public void Tick() => _timer.Tick();
    }
}

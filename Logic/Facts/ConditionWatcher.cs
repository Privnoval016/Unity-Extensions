using System;
using System.Collections.Generic;

namespace Extensions.Logic
{
    /**
     * <summary>Watches one condition over a set of facts and reports when its result changes, checking it again only
     * when something it reads changes (or after any change, when it can't say what it reads).</summary>
     * <remarks>Plain C#: a component owns one and disposes it. Locks, reactions and rewards gated on conditions all use the same shape.</remarks>
     */
    public sealed class ConditionWatcher : IDisposable
    {
        #region Private Fields

        private readonly ICondition<IFacts> _condition;
        private readonly IFacts _facts;
        private readonly Action<bool> _onChanged;
        private readonly HashSet<IConditionSubject> _reads = new HashSet<IConditionSubject>();
        private readonly bool _readsKnown;
        private bool _disposed;

        #endregion

        #region Properties

        /** <summary>The condition's result at the last check; null before the first.</summary> */
        public bool? Holds { get; private set; }

        #endregion

        #region Public API

        /**
         * <summary>Starts watching and checks the condition at once.</summary>
         * <param name="condition">The condition; null never holds.</param>
         * <param name="facts">What it reads, and where changes are announced.</param>
         * <param name="onChanged">Called with the new result whenever it changes, the first check included.</param>
         */
        public ConditionWatcher(ICondition<IFacts> condition, IFacts facts, Action<bool> onChanged)
        {
            _condition = condition;
            _facts = facts ?? throw new ArgumentNullException(nameof(facts));
            _onChanged = onChanged ?? throw new ArgumentNullException(nameof(onChanged));
            _readsKnown = ConditionSubjects.TryCollect(condition, _reads);
            _facts.OnSubjectChanged += HandleSubjectChanged;
            Refresh();
        }

        /** <summary>Checks the condition now, reporting if its result changed.</summary> */
        public void Refresh()
        {
            if (_disposed) return;
            var holds = _condition != null && _condition.Evaluate(_facts);
            if (Holds == holds) return;
            Holds = holds;
            _onChanged(holds);
        }

        /** <summary>Forgets the last result and checks again, reporting it even if unchanged: after a load, say.</summary> */
        public void Reapply()
        {
            Holds = null;
            Refresh();
        }

        /** <summary>Stops watching.</summary> */
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _facts.OnSubjectChanged -= HandleSubjectChanged;
        }

        #endregion

        #region Event Handlers

        private void HandleSubjectChanged(IConditionSubject subject)
        {
            if (!_readsKnown || _reads.Contains(subject)) Refresh();
        }

        #endregion
    }
}

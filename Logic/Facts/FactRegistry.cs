using System;
using System.Collections.Generic;

namespace Extensions.Logic
{
    /** <summary>The facts conditions read, registered by type, and the place changes are announced.</summary> */
    public sealed class FactRegistry : IFacts
    {
        #region Private Fields

        /** <summary>Keyed by the fact interface a source was registered as, resolving to the source.</summary> */
        private readonly Dictionary<Type, object> _sources = new Dictionary<Type, object>();

        #endregion

        #region Properties

        /** <inheritdoc /> */
        public event Action<IConditionSubject> OnSubjectChanged = delegate { };

        #endregion

        #region Public API

        /**
         * <summary>Registers a fact source under the type conditions ask for, usually an interface.</summary>
         * <exception cref="InvalidOperationException">A source is already registered under that type.</exception>
         */
        public void Register<T>(T facts) where T : class
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            if (_sources.ContainsKey(typeof(T))) throw new InvalidOperationException($"Facts of type {typeof(T).Name} are already registered.");
            _sources.Add(typeof(T), facts);
        }

        /** <summary>Removes the source registered under a type, if it is the given one.</summary> */
        public void Unregister<T>(T facts) where T : class
        {
            if (_sources.TryGetValue(typeof(T), out var registered) && ReferenceEquals(registered, facts)) _sources.Remove(typeof(T));
        }

        /** <inheritdoc /> */
        public bool TryGet<T>(out T facts) where T : class
        {
            facts = _sources.TryGetValue(typeof(T), out var source) ? (T)source : null;
            return facts != null;
        }

        /** <summary>Announces that a subject has changed.</summary> */
        public void NotifyChanged(IConditionSubject subject)
        {
            if (subject != null) OnSubjectChanged(subject);
        }

        #endregion
    }
}

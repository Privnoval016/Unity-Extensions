using System;

namespace Extensions.Logic
{
    /** <summary>Small fact sets for one moment, such as what an event says about itself, and layering them over the world's.</summary> */
    public static class Facts
    {
        #region Public API

        /** <summary>No facts.</summary> */
        public static IFacts None { get; } = new NoFacts();

        /** <summary>A fact set holding one fact source.</summary> */
        public static IFacts Of<T>(T facts) where T : class => facts == null ? None : new SingleFact<T>(facts);

        /** <summary>Facts read from the top set first, then the bottom one; changes are those of the bottom set.</summary> */
        public static IFacts Layered(IFacts top, IFacts bottom)
        {
            if (top == null || top == None) return bottom ?? None;
            return bottom == null ? top : new LayeredFacts(top, bottom);
        }

        #endregion

        #region Helpers

        /** <summary>No facts at all.</summary> */
        private sealed class NoFacts : IFacts
        {
            public event Action<IConditionSubject> OnSubjectChanged
            {
                add { }
                remove { }
            }

            public bool TryGet<T>(out T facts) where T : class
            {
                facts = null;
                return false;
            }
        }

        /** <summary>One fact source, found as its own type.</summary> */
        private sealed class SingleFact<TFact> : IFacts where TFact : class
        {
            private readonly TFact _fact;

            public SingleFact(TFact fact) => _fact = fact;

            public event Action<IConditionSubject> OnSubjectChanged
            {
                add { }
                remove { }
            }

            public bool TryGet<T>(out T facts) where T : class
            {
                facts = _fact as T;
                return facts != null;
            }
        }

        /** <summary>Two fact sets, the top one first.</summary> */
        private sealed class LayeredFacts : IFacts
        {
            private readonly IFacts _top;
            private readonly IFacts _bottom;

            public LayeredFacts(IFacts top, IFacts bottom)
            {
                _top = top;
                _bottom = bottom;
            }

            public event Action<IConditionSubject> OnSubjectChanged
            {
                add => _bottom.OnSubjectChanged += value;
                remove => _bottom.OnSubjectChanged -= value;
            }

            public bool TryGet<T>(out T facts) where T : class => _top.TryGet(out facts) || _bottom.TryGet(out facts);
        }

        #endregion
    }
}

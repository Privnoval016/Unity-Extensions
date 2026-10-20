using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Logic
{
    /** <summary>True when its child is false. With no child it has nothing to negate and reads as false.</summary> */
    [Serializable]
    public class NotCondition<TContext> : ICondition<TContext>, ISubjectReader, ICompositeCondition<TContext>
    {
        [SerializeReference] 
        public ICondition<TContext> child;

        public bool Evaluate(TContext context)
        {
            return child != null && !child.Evaluate(context);
        }

        /** <inheritdoc /> */
        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollect(child, subjects);

        [NonSerialized] private ICondition<TContext>[] _children;

        /** <inheritdoc /> */
        public IReadOnlyList<ICondition<TContext>> Children
        {
            get
            {
                _children ??= new ICondition<TContext>[1];
                _children[0] = child;
                return _children;
            }
        }

        /** <inheritdoc /> */
        public CompositeRule Rule => CompositeRule.Not;

        /** <summary>"not" and its part; "never" when it has none.</summary> */
        public override string ToString() => child == null ? "never" : "not " + ConditionTree.Summarize(child);
    }
}
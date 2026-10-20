using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Logic
{
    /** <summary>True when every child is; an empty slot is skipped, so no children reads as true.</summary> */
    [Serializable]
    public class AndCondition<TContext> : ICondition<TContext>, ISubjectReader, ICompositeCondition<TContext>
    {
        [SerializeReference] 
        public List<ICondition<TContext>> children = new();

        public bool Evaluate(TContext context)
        {
            foreach (var child in children)
            {
                if (child != null && !child.Evaluate(context))
                    return false;
            }
            return true;
        }
        
        /** <inheritdoc /> */
        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollectAll(children, subjects);

        /** <inheritdoc /> */
        public IReadOnlyList<ICondition<TContext>> Children => children;

        /** <inheritdoc /> */
        public CompositeRule Rule => CompositeRule.All;

        /** <summary>Its parts joined by "and", in brackets when there are several; "always" when it has none.</summary> */
        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var child in children)
                if (child != null) parts.Add(ConditionTree.Summarize(child));
            return parts.Count == 0 ? "always" : parts.Count == 1 ? parts[0] : "(" + string.Join(" and ", parts) + ")";
        }
    }
}
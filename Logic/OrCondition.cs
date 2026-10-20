using System.Collections.Generic;
using System;
using UnityEngine;

namespace Extensions.Logic
{
    /** <summary>True when any child is; an empty slot is skipped, so no children reads as false.</summary> */
    [Serializable]
    public class OrCondition<TContext> : ICondition<TContext>, ISubjectReader, ICompositeCondition<TContext>
    {
        [SerializeReference] 
        public List<ICondition<TContext>> children = new();

        public bool Evaluate(TContext context)
        {
            foreach (var child in children)
            {
                if (child != null && child.Evaluate(context))
                    return true;
            }
            return false;
        }
        
        /** <inheritdoc /> */
        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollectAll(children, subjects);

        /** <inheritdoc /> */
        public IReadOnlyList<ICondition<TContext>> Children => children;

        /** <inheritdoc /> */
        public CompositeRule Rule => CompositeRule.Any;

        /** <summary>Its parts joined by "or", in brackets when there are several; "never" when it has none.</summary> */
        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var child in children)
                if (child != null) parts.Add(ConditionTree.Summarize(child));
            return parts.Count == 0 ? "never" : parts.Count == 1 ? parts[0] : "(" + string.Join(" or ", parts) + ")";
        }
    }
}
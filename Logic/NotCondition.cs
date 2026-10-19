using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Logic
{
    /** <summary>True when its child is false. With no child it has nothing to negate and reads as false.</summary> */
    [Serializable]
    public class NotCondition<TContext> : ICondition<TContext>, ISubjectReader
    {
        [SerializeReference] 
        public ICondition<TContext> child;

        public bool Evaluate(TContext context)
        {
            return child != null && !child.Evaluate(context);
        }

        /** <inheritdoc /> */
        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollect(child, subjects);

        public override string ToString()
        {
            return "* NOT Condition *";
        }
    }
}
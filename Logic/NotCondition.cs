using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Logic
{
    [Serializable]
    public class NotCondition<TContext> : ICondition<TContext>, ISubjectReader
    {
        [SerializeReference] 
        public ICondition<TContext> child;

        public bool Evaluate(TContext context)
        {
            return !child.Evaluate(context);
        }

        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollect(child, subjects);

        public override string ToString()
        {
            return "* NOT Condition *";
        }
    }
}
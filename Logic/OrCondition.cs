using System.Collections.Generic;
using System;
using UnityEngine;

namespace Extensions.Logic
{
    [Serializable]
    public class OrCondition<TContext> : ICondition<TContext>, ISubjectReader
    {
        [SerializeReference] 
        public List<ICondition<TContext>> children = new();

        public bool Evaluate(TContext context)
        {
            foreach (var child in children)
            {
                if (child.Evaluate(context))
                    return true;
            }
            return false;
        }
        
        public bool TryCollectSubjects(ICollection<IConditionSubject> subjects) => ConditionSubjects.TryCollectAll(children, subjects);

        public override string ToString()
        {
            return "* OR Condition *";
        }
    }
}
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Extensions.Logic
{
    [Serializable]
    public class OrCondition<TContext> : ICondition<TContext>
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
        
        public override string ToString()
        {
            return "* OR Condition *";
        }
    }
}
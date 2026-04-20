using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Logic
{
    [Serializable]
    public class AndCondition<TContext> : ICondition<TContext>
    {
        [SerializeReference] 
        public List<ICondition<TContext>> children = new();

        public bool Evaluate(TContext context)
        {
            foreach (var child in children)
            {
                if (!child.Evaluate(context))
                    return false;
            }
            return true;
        }
        
        public override string ToString()
        {
            return "* AND Condition *";
        }
    }
}
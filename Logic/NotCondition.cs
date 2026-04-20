using System;
using UnityEngine;

namespace Extensions.Logic
{
    [Serializable]
    public class NotCondition<TContext> : ICondition<TContext>
    {
        [SerializeReference] 
        public ICondition<TContext> child;

        public bool Evaluate(TContext context)
        {
            return !child.Evaluate(context);
        }

        public override string ToString()
        {
            return "* NOT Condition *";
        }
    }
}
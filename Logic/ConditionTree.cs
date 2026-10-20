using System;

namespace Extensions.Logic
{
    /** <summary>How a part sits in a condition tree: read as itself or negated, and whether the whole can only hold if it does.</summary> */
    public readonly struct LeafUse
    {
        #region Properties

        /** <summary>True when the part is read as itself; false when an odd number of Nots above it negate it.</summary> */
        public bool AsItself { get; }

        /** <summary>True when the whole can only hold if the part reads as <see cref="AsItself" /> says: no Or offers another way.</summary> */
        public bool Required { get; }

        #endregion

        #region Public API

        /** <summary>Makes a use.</summary> */
        public LeafUse(bool asItself, bool required)
        {
            AsItself = asItself;
            Required = required;
        }

        #endregion
    }

    /** <summary>Walks a condition tree for tools: each part that isn't a composite, and how it is used.</summary> */
    public static class ConditionTree
    {
        #region Private Fields

        /** <summary>How deep a walk goes before it stops, so a tree that contains itself can't hang a tool.</summary> */
        private const int MaxDepth = 64;

        #endregion

        #region Public API

        /** <summary>The condition as a composite; null when it isn't one.</summary> */
        public static ICompositeCondition<TContext> CompositeOf<TContext>(ICondition<TContext> condition) => condition as ICompositeCondition<TContext>;

        /** <summary>Visits every part that isn't a composite, with how it is used. Empty slots are skipped.</summary> */
        public static void VisitLeaves<TContext>(ICondition<TContext> condition, Action<ICondition<TContext>, LeafUse> visit) => Visit(condition, true, true, 0, visit);

        /** <summary>A short summary of a condition, for tools: its own text, or its type's name when it has none; "none" for an empty slot.</summary> */
        public static string Summarize<TContext>(ICondition<TContext> condition)
        {
            if (condition == null) return "none";
            var text = condition.ToString();
            var type = condition.GetType();
            if (text != type.ToString()) return text;
            var tick = type.Name.IndexOf('`');
            return tick < 0 ? type.Name : type.Name.Substring(0, tick);
        }

        #endregion

        #region Helpers

        private static void Visit<TContext>(ICondition<TContext> condition, bool asItself, bool required, int depth, Action<ICondition<TContext>, LeafUse> visit)
        {
            if (condition == null || depth > MaxDepth) return;
            var composite = CompositeOf(condition);
            if (composite == null)
            {
                visit(condition, new LeafUse(asItself, required));
                return;
            }
            var parts = 0;
            foreach (var child in composite.Children)
                if (child != null) parts++;
            var childAsItself = composite.Rule == CompositeRule.Not ? !asItself : asItself;
            // Every part is needed when the parts all must hold as read: All read as itself, or Any negated (not (a or b) is not a and not b).
            var everyPart = composite.Rule == CompositeRule.Not || parts == 1 || (composite.Rule == CompositeRule.All) == asItself;
            foreach (var child in composite.Children)
                Visit(child, childAsItself, required && everyPart, depth + 1, visit);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        /** <summary>How deep a walk goes before it stops.</summary> */
        private const int MaxDepth = 64;

        /** <summary>How deep a summary goes before it writes "…".</summary> */
        private const int MaxSummaryDepth = 12;

        [ThreadStatic] private static int _summaryDepth;

        #endregion

        #region Public API

        /** <summary>The condition as a composite; null when it isn't one.</summary> */
        public static ICompositeCondition<TContext> CompositeOf<TContext>(ICondition<TContext> condition) => condition as ICompositeCondition<TContext>;

        /** <summary>Visits every part that isn't a composite, with how it is used. Empty slots are skipped.</summary> */
        public static void VisitLeaves<TContext>(ICondition<TContext> condition, Action<ICondition<TContext>, LeafUse> visit) =>
            Visit(condition, true, true, new HashSet<object>(ByReference.Instance), visit);

        /** <summary>A short summary of a condition, for tools: its own text, or its type's name when it has none; "none" for an empty slot.</summary> */
        public static string Summarize<TContext>(ICondition<TContext> condition)
        {
            if (condition == null) return "none";
            if (_summaryDepth >= MaxSummaryDepth) return "…";
            _summaryDepth++;
            try
            {
                var text = condition.ToString();
                var type = condition.GetType();
                if (!string.IsNullOrEmpty(text) && text != type.ToString()) return text;
                var tick = type.Name.IndexOf('`');
                return tick < 0 ? type.Name : type.Name.Substring(0, tick);
            }
            finally
            {
                _summaryDepth--;
            }
        }

        #endregion

        #region Helpers

        /** <summary>Walks a part, skipping any composite already on the path so a tree that contains itself ends.</summary> */
        private static void Visit<TContext>(ICondition<TContext> condition, bool asItself, bool required, HashSet<object> path, Action<ICondition<TContext>, LeafUse> visit)
        {
            if (condition == null || path.Count > MaxDepth) return;
            var composite = CompositeOf(condition);
            if (composite == null)
            {
                visit(condition, new LeafUse(asItself, required));
                return;
            }
            if (!path.Add(composite)) return;
            var parts = 0;
            foreach (var child in composite.Children)
                if (child != null) parts++;
            var childAsItself = composite.Rule == CompositeRule.Not ? !asItself : asItself;
            var everyPart = NeedsEveryPart(composite.Rule, parts, asItself);
            foreach (var child in composite.Children)
                Visit(child, childAsItself, required && everyPart, path, visit);
            path.Remove(composite);
        }

        /**
         * <summary>True when the whole holds only if every part reads as it is read: an All read as itself, an Any
         * negated (not (a or b) is not a and not b), a Not, or a composite of one part.</summary>
         */
        private static bool NeedsEveryPart(CompositeRule rule, int parts, bool asItself) =>
            rule == CompositeRule.Not || parts == 1 || (rule == CompositeRule.All) == asItself;

        /** <summary>Compares by reference, whatever a type's own equality says.</summary> */
        private sealed class ByReference : IEqualityComparer<object>
        {
            public static readonly ByReference Instance = new ByReference();

            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Extensions.Logic;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>Walking condition trees: how each part is used, empty slots, deep trees, and summaries.</summary> */
    public sealed class ConditionTreeTests
    {
        [Serializable]
        private sealed class Named : ICondition<int>
        {
            public string Name;

            public bool Evaluate(int context) => true;

            public override string ToString() => Name;
        }

        private static Named N(string name) => new Named { Name = name };

        private static Dictionary<string, LeafUse> Uses(ICondition<int> tree)
        {
            var uses = new Dictionary<string, LeafUse>();
            ConditionTree.VisitLeaves(tree, (leaf, use) => uses[leaf.ToString()] = use);
            return uses;
        }

        [Test]
        public void Leaves_KnowWhetherTheyAreNegated_AndWhetherTheWholeNeedsThem()
        {
            var tree = new AndCondition<int>
            {
                children =
                {
                    N("a"),
                    new OrCondition<int> { children = { N("b"), N("c") } },
                    new NotCondition<int> { child = N("d") },
                    new NotCondition<int> { child = new OrCondition<int> { children = { N("e"), N("f") } } },
                    new NotCondition<int> { child = new AndCondition<int> { children = { N("g"), N("h") } } },
                    new OrCondition<int> { children = { N("i") } },
                },
            };
            var uses = Uses(tree);
            Assert.That(uses["a"].AsItself && uses["a"].Required);
            Assert.That(uses["b"].AsItself && !uses["b"].Required, "an Or offers another way");
            Assert.That(!uses["d"].AsItself && uses["d"].Required);
            Assert.That(!uses["e"].AsItself && uses["e"].Required, "not (e or f) needs both negated");
            Assert.That(!uses["g"].AsItself && !uses["g"].Required, "not (g and h) is satisfied by either");
            Assert.That(uses["i"].Required, "an Or of one part needs it");
        }

        [Test]
        public void EmptySlots_AreSkipped_AndDeepTreesStop()
        {
            Assert.That(Uses(new AndCondition<int> { children = { null, N("a") } }).Keys, Is.EquivalentTo(new[] { "a" }));
            Assert.That(Uses(null), Is.Empty);
            ICondition<int> deep = N("bottom");
            for (var i = 0; i < 200; i++) deep = new NotCondition<int> { child = deep };
            Assert.DoesNotThrow(() => Uses(deep));
            Assert.That(Uses(deep), Is.Empty, "a tree deeper than the limit is cut off, not walked");
        }

        [Test, Timeout(5000)]
        public void ATreeThatContainsItself_IsWalkedAndSummarizedWithoutEnding()
        {
            var and = new AndCondition<int> { children = { N("a") } };
            and.children.Add(new NotCondition<int> { child = and });
            Assert.That(Uses(and).Keys, Is.EquivalentTo(new[] { "a" }), "each part once, then the loop is cut");
            Assert.That(ConditionTree.Summarize(and), Does.Contain("…"), "a summary stops at a depth");
        }

        [Test]
        public void APartWithNoText_IsSummarizedByItsType()
        {
            Assert.That(ConditionTree.Summarize(N("")), Is.EqualTo("Named"));
            Assert.That(ConditionTree.Summarize(N(null)), Is.EqualTo("Named"));
        }

        [Test]
        public void Summaries_ReadAsWords()
        {
            Assert.That(ConditionTree.Summarize(new AndCondition<int> { children = { N("a"), new OrCondition<int> { children = { N("b"), N("c") } } } }), Is.EqualTo("(a and (b or c))"));
            Assert.That(ConditionTree.Summarize(new NotCondition<int> { child = N("a") }), Is.EqualTo("not a"));
            Assert.That(ConditionTree.Summarize(new AndCondition<int>()), Is.EqualTo("always"));
            Assert.That(ConditionTree.Summarize(new OrCondition<int>()), Is.EqualTo("never"));
            Assert.That(ConditionTree.Summarize(new NotCondition<int>()), Is.EqualTo("never"));
            Assert.That(ConditionTree.Summarize<int>(null), Is.EqualTo("none"));
            Assert.That(ConditionTree.Summarize(new LeafCondition<int>(_ => true)), Is.EqualTo("LeafCondition"));
        }
    }
}

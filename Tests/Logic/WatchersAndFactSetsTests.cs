using System;
using System.Collections.Generic;
using System.ComponentModel;
using Extensions.Ids;
using Extensions.Logic;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>Watching a condition, layered fact sets, combinators with empty slots, number comparisons and id text.</summary> */
    public sealed class WatchersAndFactSetsTests
    {
        public interface ICounter
        {
            int Count { get; }
        }

        public interface IName
        {
            string Name { get; }
        }

        private sealed class Counter : ICounter
        {
            public int Count { get; set; }
        }

        private sealed class Named : IName
        {
            public Named(string name) => Name = name;

            public string Name { get; }
        }

        private sealed class Subject : IConditionSubject { }

        [Serializable]
        private sealed class CountAtLeast : ICondition<IFacts>, ISubjectReader
        {
            public Subject Reads;
            public int Number;
            public bool SaysWhatItReads = true;
            public int Checks;

            public bool Evaluate(IFacts context)
            {
                Checks++;
                return context.TryGet<ICounter>(out var counter) && counter.Count >= Number;
            }

            public bool TryCollectSubjects(ICollection<IConditionSubject> subjects)
            {
                subjects.Add(Reads);
                return SaysWhatItReads;
            }
        }

        [Serializable]
        private sealed class Fixed : ICondition<IFacts>
        {
            public bool Value;

            public bool Evaluate(IFacts context) => Value;
        }

        [Test]
        public void AWatcher_ReportsTheFirstResult_ThenOnlyChanges()
        {
            var registry = new FactRegistry();
            var counter = new Counter();
            registry.Register<ICounter>(counter);
            var subject = new Subject();
            var heard = new List<bool>();
            using var watcher = new ConditionWatcher(new CountAtLeast { Reads = subject, Number = 2 }, registry, heard.Add);

            counter.Count = 1;
            registry.NotifyChanged(subject);
            counter.Count = 2;
            registry.NotifyChanged(subject);
            registry.NotifyChanged(subject);
            Assert.That(heard, Is.EqualTo(new[] { false, true }));
            Assert.That(watcher.Holds, Is.True);
        }

        [Test]
        public void AWatcher_ChecksOnlyForWhatItReads_UnlessItCantSay()
        {
            var registry = new FactRegistry();
            var mine = new Subject();
            var known = new CountAtLeast { Reads = mine };
            var unknown = new CountAtLeast { Reads = mine, SaysWhatItReads = false };
            using var a = new ConditionWatcher(known, registry, _ => { });
            using var b = new ConditionWatcher(unknown, registry, _ => { });
            var knownBefore = known.Checks;
            var unknownBefore = unknown.Checks;

            registry.NotifyChanged(new Subject());
            Assert.That(known.Checks, Is.EqualTo(knownBefore));
            Assert.That(unknown.Checks, Is.EqualTo(unknownBefore + 1));
            registry.NotifyChanged(mine);
            Assert.That(known.Checks, Is.EqualTo(knownBefore + 1));
        }

        [Test]
        public void AWatcher_ReappliesOnRequest_AndStopsWhenDisposed()
        {
            var registry = new FactRegistry();
            var subject = new Subject();
            var heard = new List<bool>();
            registry.Register<ICounter>(new Counter());
            var watcher = new ConditionWatcher(new CountAtLeast { Reads = subject }, registry, heard.Add);
            watcher.Reapply();
            Assert.That(heard, Is.EqualTo(new[] { true, true }));

            watcher.Dispose();
            watcher.Dispose();
            registry.NotifyChanged(subject);
            watcher.Refresh();
            Assert.That(heard, Has.Count.EqualTo(2));
        }

        [Test]
        public void AWatcher_WithNoCondition_NeverHolds_AndNeedsFactsAndAListener()
        {
            var registry = new FactRegistry();
            var heard = new List<bool>();
            using var watcher = new ConditionWatcher(null, registry, heard.Add);
            Assert.That(heard, Is.EqualTo(new[] { false }));
            Assert.Throws<ArgumentNullException>(() => new ConditionWatcher(null, null, heard.Add));
            Assert.Throws<ArgumentNullException>(() => new ConditionWatcher(null, registry, null));
        }

        [Test]
        public void LayeredFacts_ReadTheTopFirst_ThenTheBottom()
        {
            var registry = new FactRegistry();
            registry.Register<IName>(new Named("world"));
            registry.Register<ICounter>(new Counter { Count = 4 });
            var layered = Facts.Layered(Facts.Of<IName>(new Named("payload")), registry);

            Assert.That(layered.TryGet<IName>(out var name) && name.Name == "payload");
            Assert.That(layered.TryGet<ICounter>(out var counter) && counter.Count == 4);
            Assert.That(Facts.Layered(null, registry), Is.SameAs(registry));
            Assert.That(Facts.Layered(Facts.None, registry), Is.SameAs(registry));
            Assert.That(Facts.Layered(null, null), Is.SameAs(Facts.None));
        }

        [Test]
        public void LayeredFacts_AnnounceTheBottomsChanges()
        {
            var registry = new FactRegistry();
            var heard = new List<IConditionSubject>();
            var layered = Facts.Layered(Facts.Of<IName>(new Named("payload")), registry);
            layered.OnSubjectChanged += heard.Add;
            var subject = new Subject();
            registry.NotifyChanged(subject);
            layered.OnSubjectChanged -= heard.Add;
            registry.NotifyChanged(subject);
            Assert.That(heard, Is.EqualTo(new[] { subject }));
        }

        [Test]
        public void OneFact_IsFoundAsItsOwnTypes_AndNothingElse()
        {
            var facts = Facts.Of<IName>(new Named("x"));
            Assert.That(facts.TryGet<IName>(out _));
            Assert.That(facts.TryGet<ICounter>(out var missing), Is.False);
            Assert.That(missing, Is.Null);
            Assert.That(Facts.Of<IName>(null), Is.SameAs(Facts.None));
            Assert.That(Facts.None.TryGet<IName>(out _), Is.False);
        }

        [Test]
        public void Combinators_SkipEmptySlots()
        {
            var facts = Facts.None;
            var yes = new Fixed { Value = true };
            Assert.That(new AndCondition<IFacts> { children = { null, yes } }.Evaluate(facts));
            Assert.That(new AndCondition<IFacts> { children = { null } }.Evaluate(facts));
            Assert.That(new OrCondition<IFacts> { children = { null, yes } }.Evaluate(facts));
            Assert.That(new OrCondition<IFacts> { children = { null } }.Evaluate(facts), Is.False);
            Assert.That(new NotCondition<IFacts>().Evaluate(facts), Is.False);
            Assert.That(new NotCondition<IFacts> { child = new Fixed() }.Evaluate(facts));
            Assert.That(ConditionSubjects.TryCollect(new AndCondition<IFacts> { children = { null } }, new HashSet<IConditionSubject>()));
        }

        [TestCase(NumberComparison.AtLeast, 3, 3, true)]
        [TestCase(NumberComparison.AtLeast, 2, 3, false)]
        [TestCase(NumberComparison.Equal, 3, 3, true)]
        [TestCase(NumberComparison.Equal, 4, 3, false)]
        [TestCase(NumberComparison.Below, 2, 3, true)]
        [TestCase(NumberComparison.Below, 3, 3, false)]
        [TestCase(NumberComparison.Below, int.MinValue, int.MinValue, false)]
        [TestCase(NumberComparison.AtLeast, int.MaxValue, int.MaxValue, true)]
        public void NumberComparisons_Hold(NumberComparison comparison, int value, int number, bool holds) =>
            Assert.That(NumberComparisons.Holds(comparison, value, number), Is.EqualTo(holds));

        [Test]
        public void AssetIds_ConvertToAndFromTheirText()
        {
            var converter = TypeDescriptor.GetConverter(typeof(AssetId));
            var id = AssetId.New();
            Assert.That(converter.ConvertToInvariantString(id), Is.EqualTo(id.Value));
            Assert.That(converter.ConvertFromInvariantString(id.Value), Is.EqualTo(id));
            Assert.That(((AssetId)converter.ConvertFromInvariantString("not an id")).IsEmpty);
            Assert.That(converter.ConvertToInvariantString(default(AssetId)), Is.Empty);
        }
    }
}

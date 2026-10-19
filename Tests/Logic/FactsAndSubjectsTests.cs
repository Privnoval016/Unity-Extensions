using System;
using System.Collections.Generic;
using Extensions.EditorTools;
using Extensions.Logic;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>The shared facts conditions read, and conditions reporting what they read.</summary> */
    public sealed class FactsAndSubjectsTests
    {
        public interface ITestFacts
        {
            int Value { get; }
        }

        private sealed class TestFacts : ITestFacts
        {
            public int Value { get; set; }
        }

        private sealed class Subject : IConditionSubject { }

        [Serializable]
        private sealed class ReadsSubject : ICondition<IFacts>, ISubjectReader
        {
            public Subject Reads;

            public bool Evaluate(IFacts context) => context.TryGet<ITestFacts>(out var facts) && facts.Value > 0;

            public bool TryCollectSubjects(ICollection<IConditionSubject> subjects)
            {
                subjects.Add(Reads);
                return true;
            }
        }

        [Serializable]
        private sealed class SaysNothing : ICondition<IFacts>
        {
            public bool Evaluate(IFacts context) => true;
        }

        [Test]
        public void Registered_FactsAreFound_ByTheTypeTheyWereRegisteredAs()
        {
            var registry = new FactRegistry();
            var facts = new TestFacts { Value = 3 };
            registry.Register<ITestFacts>(facts);

            Assert.That(registry.TryGet<ITestFacts>(out var found) && found == facts);
            Assert.That(registry.TryGet<TestFacts>(out _), Is.False);
        }

        [Test]
        public void MissingFacts_ReadAsFalse_AndConditionsSeeNothing()
        {
            var registry = new FactRegistry();
            Assert.That(registry.TryGet<ITestFacts>(out var found), Is.False);
            Assert.That(found, Is.Null);
            Assert.That(new ReadsSubject().Evaluate(registry), Is.False);
        }

        [Test]
        public void RegisteringTwice_Throws_AndUnregisteringAnotherSource_DoesNothing()
        {
            var registry = new FactRegistry();
            var facts = new TestFacts();
            registry.Register<ITestFacts>(facts);
            Assert.Throws<InvalidOperationException>(() => registry.Register<ITestFacts>(new TestFacts()));
            registry.Unregister<ITestFacts>(new TestFacts());
            Assert.That(registry.TryGet<ITestFacts>(out _));
            registry.Unregister<ITestFacts>(facts);
            Assert.That(registry.TryGet<ITestFacts>(out _), Is.False);
            Assert.Throws<ArgumentNullException>(() => registry.Register<ITestFacts>(null));
        }

        [Test]
        public void Changes_AreAnnounced_AndNullIsIgnored()
        {
            var registry = new FactRegistry();
            var heard = new List<IConditionSubject>();
            registry.OnSubjectChanged += heard.Add;
            var subject = new Subject();
            registry.NotifyChanged(subject);
            registry.NotifyChanged(null);
            Assert.That(heard, Is.EqualTo(new[] { subject }));
        }

        [Test]
        public void AndOrNot_CollectTheirChildrensSubjects_AtAnyDepth()
        {
            var a = new Subject();
            var b = new Subject();
            var c = new Subject();
            var tree = new AndCondition<IFacts>
            {
                children =
                {
                    new ReadsSubject { Reads = a },
                    new OrCondition<IFacts> { children = { new ReadsSubject { Reads = b }, new NotCondition<IFacts> { child = new ReadsSubject { Reads = c } } } },
                },
            };

            var subjects = new HashSet<IConditionSubject>();
            Assert.That(ConditionSubjects.TryCollect(tree, subjects));
            Assert.That(subjects, Is.EquivalentTo(new IConditionSubject[] { a, b, c }));
        }

        [Test]
        public void APartThatSaysNothing_MakesTheWholeUnknown()
        {
            var tree = new AndCondition<IFacts> { children = { new ReadsSubject { Reads = new Subject() }, new OrCondition<IFacts> { children = { new SaysNothing() } } } };
            var subjects = new HashSet<IConditionSubject>();
            Assert.That(ConditionSubjects.TryCollect(tree, subjects), Is.False);
            Assert.That(subjects, Has.Count.EqualTo(1));
        }

        [Test]
        public void NoCondition_ReadsNothing_AndIsKnown()
        {
            var subjects = new HashSet<IConditionSubject>();
            Assert.That(ConditionSubjects.TryCollect<IFacts>(null, subjects));
            Assert.That(new NotCondition<IFacts>().TryCollectSubjects(subjects));
            Assert.That(subjects, Is.Empty);
        }

        [Test]
        public void ThePicker_OffersOnlyCreatableTypes()
        {
            var types = SubclassTypes.For(typeof(ICondition<IFacts>));
            Assert.That(types, Does.Contain(typeof(ReadsSubject)));
            Assert.That(types, Does.Contain(typeof(AndCondition<IFacts>)));
            Assert.That(types, Does.Contain(typeof(OrCondition<IFacts>)));
            Assert.That(types, Does.Contain(typeof(NotCondition<IFacts>)));
            Assert.That(types, Has.No.Member(typeof(LeafCondition<IFacts>)));
            Assert.That(types, Has.None.Matches<Type>(type => type.IsAbstract || type.IsInterface || type.ContainsGenericParameters));
            Assert.That(SubclassTypes.For(typeof(ICondition<int>)), Has.No.Member(typeof(ReadsSubject)));
            Assert.That(SubclassTypes.For(null), Is.Empty);
        }

        [Test]
        public void ThePicker_ReadsUnityTypeNames()
        {
            Assert.That(SubclassTypes.FromManagedReferenceName("Extensions Extensions.Logic.FactRegistry"), Is.EqualTo(typeof(FactRegistry)));
            Assert.That(SubclassTypes.FromManagedReferenceName(""), Is.Null);
            Assert.That(SubclassTypes.FromManagedReferenceName("NoSpace"), Is.Null);
            Assert.That(SubclassTypes.Label(null), Is.EqualTo("None"));
        }
    }
}

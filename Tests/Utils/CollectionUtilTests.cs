using System.Collections.Generic;
using Extensions.Utils;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>A list's reference membership: the same object only, never an equal one, and nothing for no list.</summary> */
    public sealed class CollectionUtilTests
    {
        private sealed class Equal
        {
            public override bool Equals(object other) => other is Equal;

            public override int GetHashCode() => 0;
        }

        [Test]
        public void Has_FindsTheSameObject_NotAnEqualOne()
        {
            var held = new Equal();
            IReadOnlyList<Equal> items = new List<Equal> { held };

            Assert.That(items.Has(held), Is.True);
            Assert.That(items.Has(new Equal()), Is.False, "equal by Equals, but not the same object");
        }

        [Test]
        public void Has_OnAnArray_AndOnEmptyOrNoList()
        {
            var a = new object();
            var b = new object();

            Assert.That(new[] { a, b }.Has(b), Is.True);
            Assert.That(new object[0].Has(a), Is.False);
            Assert.That(((IReadOnlyList<object>)null).Has(a), Is.False);
        }

        [Test]
        public void Has_Null_FindsOnlyANullEntry()
        {
            Assert.That(new object[] { null }.Has(null), Is.True);
            Assert.That(new[] { new object() }.Has(null), Is.False);
        }
    }
}

using System.Linq;
using Extensions.EditorTools;
using Extensions.Ids;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>The id index the postprocessor uses to avoid project scans.</summary> */
    public sealed class AssetIdIndexTests
    {
        private static readonly AssetId A = AssetId.New();
        private static readonly AssetId B = AssetId.New();

        [Test]
        public void Record_ThenFind()
        {
            var index = new AssetIdIndex();
            index.Record("x.asset", new[] { A, B });
            index.Record("y.asset", new[] { A });
            Assert.That(index.Contains(A));
            Assert.That(index.PathsHolding(new[] { A }), Is.EquivalentTo(new[] { "x.asset", "y.asset" }));
            Assert.That(index.PathsHolding(new[] { A, B }).Count(), Is.EqualTo(2));
        }

        [Test]
        public void Rerecording_ReplacesAFilesIds()
        {
            var index = new AssetIdIndex();
            index.Record("x.asset", new[] { A });
            index.Record("x.asset", new[] { B });
            Assert.That(index.Contains(A), Is.False);
            Assert.That(index.PathsHolding(new[] { B }), Is.EqualTo(new[] { "x.asset" }));
        }

        [Test]
        public void Forget_RemovesAFile_AndEmptyIdsAreIgnored()
        {
            var index = new AssetIdIndex();
            index.Record("x.asset", new[] { A, default });
            index.Forget("x.asset");
            index.Forget("never.asset");
            Assert.That(index.Contains(A), Is.False);
            Assert.That(index.Contains(default), Is.False);
        }
    }
}

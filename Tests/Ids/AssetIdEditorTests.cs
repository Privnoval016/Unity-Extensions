using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Extensions.EditorTools;
using Extensions.Ids;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions.Tests
{
    /** <summary>Ids, the catalog and the audit on real assets.</summary> */
    public sealed class AssetIdEditorTests
    {
        private const string Folder = "Assets/SaveSystemTestTemp";

        [SetUp]
        public void SetUp()
        {
            if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets", "SaveSystemTestTemp");
        }

        [TearDown]
        public void TearDown() => AssetDatabase.DeleteAsset(Folder);

        private static T Create<T>(string name) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{Folder}/{name}.asset");
            return asset;
        }

        private static void WriteAndSave(Object asset, AssetId id)
        {
            IdentifiedAssetDatabase.WriteId(asset, id);
            AssetDatabase.SaveAssetIfDirty(asset);
        }

        private static string GuidOf(Object asset) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));

        [Test]
        public void NewAsset_GetsItsOwnGuid()
        {
            var asset = Create<TestIdentifiedAsset>("New");
            AssetIdFixer.FixAll();
            Assert.That(asset.AssetId.Value, Is.EqualTo(GuidOf(asset)));
        }

        [Test]
        public void DuplicatedAsset_GetsAFreshId_AndTheOriginalKeepsItsOwn()
        {
            var original = Create<TestIdentifiedAsset>("Original");
            AssetIdFixer.FixAll();
            var originalId = original.AssetId;
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(original), $"{Folder}/Copy.asset");
            var copy = AssetDatabase.LoadAssetAtPath<TestIdentifiedAsset>($"{Folder}/Copy.asset");
            Assert.That(copy.AssetId, Is.EqualTo(originalId));

            AssetIdFixer.FixAll(new[] { $"{Folder}/Copy.asset" });

            Assert.That(original.AssetId, Is.EqualTo(originalId));
            Assert.That(copy.AssetId.Value, Is.EqualTo(GuidOf(copy)));
        }

        [Test]
        public void StoredIdNotMatchingItsGuid_IsKept()
        {
            var asset = Create<TestIdentifiedAsset>("Regenerated");
            var stored = AssetId.New();
            WriteAndSave(asset, stored);

            AssetIdFixer.FixAll(new[] { AssetDatabase.GetAssetPath(asset) });
            Assert.That(asset.AssetId, Is.EqualTo(stored));
        }

        [Test]
        public void SubAsset_GetsItsOwnRandomId()
        {
            var main = Create<TestIdentifiedAsset>("Main");
            var sub = ScriptableObject.CreateInstance<TestIdentifiedAsset>();
            sub.name = "Sub";
            AssetDatabase.AddObjectToAsset(sub, main);
            AssetDatabase.SaveAssets();

            AssetIdFixer.FixAll();
            Assert.That(main.AssetId.Value, Is.EqualTo(GuidOf(main)));
            Assert.That(sub.AssetId.IsEmpty, Is.False);
            Assert.That(sub.AssetId, Is.Not.EqualTo(main.AssetId));
        }

        [Test]
        public void FixedId_IsSavedToDisk()
        {
            var asset = Create<TestIdentifiedAsset>("Saved");
            AssetIdFixer.FixAll();
            var path = AssetDatabase.GetAssetPath(asset);
            Assert.That(File.ReadAllText(path), Does.Contain(asset.AssetId.Value));
        }

        [Test]
        public void Catalog_ListsAssets_AndDropsDeletedOnes()
        {
            var catalog = Create<AssetCatalog>("Catalog");
            var a = Create<TestIdentifiedAsset>("A");
            var b = Create<OtherIdentifiedAsset>("B");
            AssetIdFixer.FixAll();
            AssetCatalogBuilder.Rebuild(new[] { catalog }, IdentifiedAssetDatabase.FindAll());

            Assert.That(catalog.Assets, Does.Contain(a));
            Assert.That(catalog.Assets, Does.Contain(b));
            Assert.That(catalog.Assets.Select(asset => AssetIds.IdOf(asset).Value), Is.Ordered.Using((IComparer<string>)StringComparer.Ordinal));
            Assert.That(catalog.TryFind(a.AssetId, out var found) && found == a);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(b));
            AssetCatalogBuilder.Rebuild(new[] { catalog }, IdentifiedAssetDatabase.FindAll());
            Assert.That(catalog.Assets, Does.Contain(a));
            Assert.That(catalog.Assets.Count(asset => asset == null), Is.Zero);
        }

        [Test]
        public void Audit_FindsMissingDuplicateAndBadFields()
        {
            var catalog = Create<AssetCatalog>("Catalog");
            var missing = Create<TestIdentifiedAsset>("Missing");
            var first = Create<TestIdentifiedAsset>("First");
            var second = Create<TestIdentifiedAsset>("Second");
            var shared = AssetId.New();
            WriteAndSave(first, shared);
            WriteAndSave(second, shared);
            var twoFields = Create<TwoIdFieldsAsset>("TwoFields");

            var issues = AssetIdAudit.Run().Where(issue => AssetDatabase.GetAssetPath(issue.Asset).StartsWith(Folder, StringComparison.Ordinal)).ToList();

            Assert.That(issues.Any(issue => issue.Problem == AssetIdProblem.Missing && issue.Asset == missing));
            Assert.That(issues.Count(issue => issue.Problem == AssetIdProblem.Duplicate), Is.EqualTo(2), string.Join(", ", issues));
            Assert.That(issues.Any(issue => issue.Problem == AssetIdProblem.NoSingleIdField && issue.Asset == twoFields));
            Assert.That(issues.Any(issue => issue.Problem == AssetIdProblem.NotInCatalog && issue.Asset == first));
            Assert.That(catalog.Assets, Is.Empty);
            Assert.That(IdentifiedAssetDatabase.WriteId(twoFields, AssetId.New()), Is.False);
        }

        [Test]
        public void Audit_IsCleanAfterFixing()
        {
            Create<AssetCatalog>("Catalog");
            var first = Create<TestIdentifiedAsset>("First");
            var second = Create<TestIdentifiedAsset>("Second");
            var shared = AssetId.New();
            WriteAndSave(first, shared);
            WriteAndSave(second, shared);

            var catalog = AssetDatabase.LoadAssetAtPath<AssetCatalog>($"{Folder}/Catalog.asset");
            AssetIdFixer.FixAll();
            AssetCatalogBuilder.Rebuild(new[] { catalog }, IdentifiedAssetDatabase.FindAll());

            var issues = AssetIdAudit.Run(new[] { catalog }).Where(issue => AssetDatabase.GetAssetPath(issue.Asset).StartsWith(Folder, StringComparison.Ordinal)).ToList();
            Assert.That(issues, Is.Empty, string.Join("\n", issues));
        }

        [Test]
        public void Fixing_Twice_ChangesNothingTheSecondTime()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TestIdentifiedAsset>(), $"{Folder}/A.asset");
            AssetIdFixer.FixAll();
            Assert.That(AssetIdFixer.FixAll(), Is.Empty);
        }

        [Test]
        public void BuildCheck_FailsOnAnAssetWithTwoIdFields()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TwoIdFieldsAsset>(), $"{Folder}/Two.asset");
            Assert.Throws<BuildFailedException>(() => new AssetIdBuildCheck().OnPreprocessBuild(null));
        }
    }
}

using System;
using System.Linq;
using Extensions.EditorTools;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Extensions.Tests
{
    /** <summary>Finding assets by type and making folders, on real assets.</summary> */
    public sealed class AssetQueryTests
    {
        private const string Folder = "Assets/AssetQueryTestTemp";

        /** <summary>Before and after each test, so a run that died midway leaves nothing for the next to find.</summary> */
        [SetUp, TearDown]
        public void Clear() => AssetDatabase.DeleteAsset(Folder);

        private static T Create<T>(string folder, string name) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{AssetQuery.EnsureFolder(folder)}/{name}.asset");
            return asset;
        }

        [Test]
        public void All_FindsEveryAssetOfTheType_InTheFoldersGiven()
        {
            var first = Create<TestIdentifiedAsset>(Folder, "First");
            var second = Create<TestIdentifiedAsset>($"{Folder}/Inner", "Second");
            Create<OtherIdentifiedAsset>(Folder, "Other");

            Assert.That(AssetQuery.All<TestIdentifiedAsset>(Folder), Is.EquivalentTo(new[] { first, second }));
            Assert.That(AssetQuery.All<TestIdentifiedAsset>($"{Folder}/Inner"), Is.EquivalentTo(new[] { second }));
        }

        [Test]
        public void All_WithNoFolders_SearchesTheWholeProject()
        {
            var made = Create<TestIdentifiedAsset>(Folder, "Anywhere");

            Assert.That(AssetQuery.All<TestIdentifiedAsset>().Contains(made), Is.True);
        }

        [Test]
        public void First_IsAnAssetOfTheType_OrNullForNone()
        {
            AssetQuery.EnsureFolder(Folder);
            Assert.That(AssetQuery.First<TestIdentifiedAsset>(Folder), Is.Null);

            var made = Create<TestIdentifiedAsset>(Folder, "Only");
            Assert.That(AssetQuery.First<TestIdentifiedAsset>(Folder), Is.EqualTo(made));
        }

        [Test]
        public void EnsureFolder_MakesMissingParents_AndKeepsWhatExists()
        {
            var deep = $"{Folder}/A/B";

            Assert.That(AssetQuery.EnsureFolder(deep), Is.EqualTo(deep));
            Assert.That(AssetDatabase.IsValidFolder(deep), Is.True);
            Assert.That(AssetQuery.EnsureFolder(deep), Is.EqualTo(deep));
            Assert.That(AssetDatabase.GetSubFolders($"{Folder}/A"), Is.EqualTo(new[] { deep }));
        }

        [Test]
        public void EnsureFolder_RefusesAFolderOutsideTheProject()
        {
            Assert.Throws<ArgumentException>(() => AssetQuery.EnsureFolder("NotAssets/Folder"));
        }
    }
}

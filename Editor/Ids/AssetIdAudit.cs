using System.Collections.Generic;
using System.Linq;
using Extensions.Ids;
using Extensions.Menus;
using UnityEditor;
using UnityEngine;

namespace Extensions.EditorTools
{
    /** <summary>What is wrong with a identified asset's id.</summary> */
    public enum AssetIdProblem
    {
        /** <summary>It has no id yet.</summary> */
        Missing,
        /** <summary>Another asset has the same id.</summary> */
        Duplicate,
        /** <summary>It doesn't have exactly one serialized <see cref="AssetId" /> field.</summary> */
        NoSingleIdField,
        /** <summary>A catalog doesn't list it.</summary> */
        NotInCatalog,
    }

    /** <summary>One problem found by the audit.</summary> */
    public readonly struct AssetIdIssue
    {
        #region Properties

        /** <summary>What is wrong.</summary> */
        public AssetIdProblem Problem { get; }

        /** <summary>The asset it is wrong with.</summary> */
        public ScriptableObject Asset { get; }

        #endregion

        #region Public API

        /** <summary>Makes an issue.</summary> */
        public AssetIdIssue(AssetIdProblem problem, ScriptableObject asset)
        {
            Problem = problem;
            Asset = asset;
        }

        /** <inheritdoc /> */
        public override string ToString() => $"{Problem}: {AssetDatabase.GetAssetPath(Asset)} ({(Asset == null ? "missing" : Asset.name)})";

        #endregion
    }

    /** <summary>Checks every identified asset: one id field, an id, a unique id, and listed in every catalog.</summary> */
    public static class AssetIdAudit
    {
        #region Public API

        /** <summary>Runs the audit against every catalog in the project.</summary> */
        public static List<AssetIdIssue> Run() => Run(AssetCatalogBuilder.FindCatalogs());

        /** <summary>Runs the audit, checking that every identified asset is listed in each of the given catalogs.</summary> */
        public static List<AssetIdIssue> Run(IEnumerable<AssetCatalog> catalogs)
        {
            var assets = IdentifiedAssetDatabase.FindAll();
            var issues = new List<AssetIdIssue>();
            foreach (var asset in assets)
            {
                if (IdentifiedAssetDatabase.CountIdFields(asset) != 1) issues.Add(new AssetIdIssue(AssetIdProblem.NoSingleIdField, asset));
                else if (!AssetIds.TryGetId(asset, out _)) issues.Add(new AssetIdIssue(AssetIdProblem.Missing, asset));
            }

            foreach (var group in assets.Where(asset => AssetIds.TryGetId(asset, out _)).GroupBy(AssetIds.IdOf).Where(group => group.Count() > 1))
                issues.AddRange(group.Select(asset => new AssetIdIssue(AssetIdProblem.Duplicate, asset)));

            foreach (var catalog in catalogs)
            {
                var listed = new HashSet<ScriptableObject>(catalog.Assets);
                issues.AddRange(assets.Where(asset => AssetIds.TryGetId(asset, out _) && !listed.Contains(asset))
                    .Select(asset => new AssetIdIssue(AssetIdProblem.NotInCatalog, asset)));
            }
            return issues;
        }

        #endregion

        #region Helpers

        [MenuItem(AssetIdMenus.Tools + "Audit Asset Ids", priority = ToolMenus.Commands + 1)]
        private static void RunFromMenu()
        {
            var issues = Run();
            if (issues.Count == 0) Debug.Log("Asset ids: asset ids are clean.");
            foreach (var issue in issues)
                Debug.LogWarning($"Asset ids: {issue}", issue.Asset);
        }

        #endregion
    }
}

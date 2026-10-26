using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Extensions.EditorTools
{
    /** <summary>Finds the project's assets of a type, and makes the folders new ones go in.</summary> */
    public static class AssetQuery
    {
        #region Public API

        /** <summary>Every asset of type <typeparamref name="T" />, in <paramref name="folders" /> if any are given, else anywhere.</summary> */
        public static IEnumerable<T> All<T>(params string[] folders) where T : Object
        {
            var filter = "t:" + typeof(T).Name;
            var guids = folders is { Length: > 0 } ? AssetDatabase.FindAssets(filter, folders) : AssetDatabase.FindAssets(filter);
            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null) yield return asset;
            }
        }

        /** <summary>The first asset of type <typeparamref name="T" />, in <paramref name="folders" /> if any are given; null for none.</summary> */
        public static T First<T>(params string[] folders) where T : Object
        {
            foreach (var asset in All<T>(folders)) return asset;
            return null;
        }

        /** <summary>Makes <paramref name="folder" /> and any missing parents, under Assets or a package.</summary> <returns>The folder.</returns> */
        public static string EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return folder;

            var parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(parent)) throw new ArgumentException($"{folder} is not under Assets or a package.", nameof(folder));
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
            return folder;
        }

        #endregion
    }
}

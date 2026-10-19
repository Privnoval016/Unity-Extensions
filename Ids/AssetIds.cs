using UnityEngine;

namespace Extensions.Ids
{
    /** <summary>The one place that asks whether an asset has an id, and reads it.</summary> */
    public static class AssetIds
    {
        #region Public API

        /** <summary>True for a live asset that implements <see cref="IIdentifiedAsset" />.</summary> */
        public static bool IsIdentified(Object asset) => asset != null && asset is IIdentifiedAsset;

        /** <summary>The asset's id; empty when it has none or isn't saveable.</summary> */
        public static AssetId IdOf(Object asset) => IsIdentified(asset) ? ((IIdentifiedAsset)asset).AssetId : default;

        /** <summary>Reads the asset's id when it is a live identified asset with an id.</summary> */
        public static bool TryGetId(Object asset, out AssetId id)
        {
            id = IdOf(asset);
            return !id.IsEmpty;
        }

        #endregion
    }
}

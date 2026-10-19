using Extensions.Ids;
using UnityEngine;

namespace Extensions.Tests
{
    /** <summary>A identified asset type for tests.</summary> */
    public sealed class TestIdentifiedAsset : ScriptableObject, IIdentifiedAsset
    {
        [Tooltip("The asset's id.")]
        [SerializeField] private AssetId assetId;

        public AssetId AssetId => assetId;

        public void SetId(AssetId id) => assetId = id;
    }
}

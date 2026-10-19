using Extensions.Ids;
using UnityEngine;

namespace Extensions.Tests
{
    /** <summary>A identified asset with two id fields, which the audit must reject.</summary> */
    public sealed class TwoIdFieldsAsset : ScriptableObject, IIdentifiedAsset
    {
        [Tooltip("The asset's id.")]
        [SerializeField] private AssetId assetId;

        [Tooltip("A second id, which is not allowed.")]
        [SerializeField] private AssetId otherId;

        public AssetId AssetId => assetId;
    }
}

namespace Extensions.Ids
{
    /**
     * <summary>A ScriptableObject other data can point at by id. It holds one serialized <see cref="AssetId" /> field, which the
     * editor fills in and keeps unique.</summary>
     */
    public interface IIdentifiedAsset
    {
        /** <summary>The asset's id. Empty until the editor assigns it.</summary> */
        AssetId AssetId { get; }
    }
}

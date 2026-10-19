using Object = UnityEngine.Object;

namespace Extensions.Ids
{
    /** <summary>Finds a identified asset from its id, in a build as well as in the editor.</summary> */
    public interface IAssetResolver
    {
        /** <summary>Finds the asset with this id.</summary> <returns>False when no asset has it.</returns> */
        bool TryFind(AssetId id, out Object asset);
    }
}

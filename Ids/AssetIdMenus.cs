using Extensions.Menus;

namespace Extensions.Ids
{
    /** <summary>Where the asset id tools sit: <i>Asset Ids</i> under the Privnoval heading.</summary> */
    public static class AssetIdMenus
    {
        #region Public API

        /** <summary>The tool's name in every menu.</summary> */
        public const string Name = "Asset Ids/";

        /** <summary>Its windows and commands.</summary> */
        public const string Tools = ToolMenus.Tools + Name;

        /** <summary>Its assets.</summary> */
        public const string Create = ToolMenus.Create + Name;

        #endregion
    }
}

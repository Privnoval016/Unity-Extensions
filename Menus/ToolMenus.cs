namespace Extensions.Menus
{
    /**
     * <summary>Where Privnoval tools put their menu entries: each tool under its own name beneath one Privnoval heading,
     * the same in the Tools, Create and Add Component menus.</summary>
     */
    public static class ToolMenus
    {
        #region Public API

        /** <summary>The heading every Privnoval tool sits under.</summary> */
        public const string Root = "Privnoval/";

        /** <summary>Windows and commands: <i>Tools &gt; Privnoval</i>.</summary> */
        public const string Tools = "Tools/" + Root;

        /** <summary>Assets: <i>Create &gt; Privnoval</i>.</summary> */
        public const string Create = Root;

        /** <summary>Components: <i>Add Component &gt; Privnoval</i>.</summary> */
        public const string Component = Root;

        /** <summary>The priority of a tool's windows, listed first.</summary> */
        public const int Windows = 0;

        /** <summary>The priority of a tool's commands, listed after its windows with a separator between.</summary> */
        public const int Commands = 20;

        #endregion
    }
}

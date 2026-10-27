using System;
using System.Collections.Generic;

namespace Extensions.Utils
{
    /** <summary>Flags on the command line the game or Editor was started with (<c>-flag value</c>).</summary> */
    public static class CommandLineUtil
    {
        #region Public API

        /** <summary>The value after <paramref name="flag" /> on this process's command line; null without one.</summary> */
        public static string ValueOf(string flag) => ValueOf(flag, Environment.GetCommandLineArgs());

        /** <summary>The value after <paramref name="flag" /> among <paramref name="arguments" />; null when the flag is missing or last.</summary> */
        public static string ValueOf(string flag, IReadOnlyList<string> arguments)
        {
            for (var i = 0; i < arguments.Count - 1; i++)
                if (arguments[i] == flag) return arguments[i + 1];
            return null;
        }

        /** <summary>Whether <paramref name="flag" /> is on this process's command line.</summary> */
        public static bool Has(string flag) => Has(flag, Environment.GetCommandLineArgs());

        /** <summary>Whether <paramref name="flag" /> is among <paramref name="arguments" />.</summary> */
        public static bool Has(string flag, IReadOnlyList<string> arguments)
        {
            for (var i = 0; i < arguments.Count; i++)
                if (arguments[i] == flag) return true;
            return false;
        }

        #endregion
    }
}

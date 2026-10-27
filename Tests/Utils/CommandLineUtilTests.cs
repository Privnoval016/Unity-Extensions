using Extensions.Utils;
using NUnit.Framework;

namespace Extensions.Tests
{
    /** <summary>Reading flags off a command line: the value after a flag, none for a missing or last one, and presence.</summary> */
    public sealed class CommandLineUtilTests
    {
        private static readonly string[] Arguments = { "Game", "-out", "Temp/a", "-il2cpp", "-mode" };

        [Test]
        public void ValueOf_IsTheArgumentAfterTheFlag()
        {
            Assert.That(CommandLineUtil.ValueOf("-out", Arguments), Is.EqualTo("Temp/a"));
            Assert.That(CommandLineUtil.ValueOf("-il2cpp", Arguments), Is.EqualTo("-mode"), "whatever follows it");
        }

        [Test]
        public void ValueOf_AMissingFlag_OrOneLast_IsNull()
        {
            Assert.That(CommandLineUtil.ValueOf("-gone", Arguments), Is.Null);
            Assert.That(CommandLineUtil.ValueOf("-mode", Arguments), Is.Null);
            Assert.That(CommandLineUtil.ValueOf("-out", new string[0]), Is.Null);
        }

        [Test]
        public void Has_IsWhetherTheFlagIsThere_LastIncluded()
        {
            Assert.That(CommandLineUtil.Has("-il2cpp", Arguments), Is.True);
            Assert.That(CommandLineUtil.Has("-mode", Arguments), Is.True);
            Assert.That(CommandLineUtil.Has("-gone", Arguments), Is.False);
        }
    }
}

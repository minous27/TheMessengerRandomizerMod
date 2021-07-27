using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessengerRando.Utils;
using Mod.Courier;
using Mod.Courier.Module;

namespace TheMessengerRandomizerTest
{
    [TestClass]
    public class ItemRandomizerUtilTest
    {
        [TestMethod]
        public void TestGetMod()
        {
            CourierModuleMetadata mod = new CourierModuleMetadata();
            mod.Name = "TheMessengerRandomizer";
            mod.VersionString = "3.5";
            Courier.Mods.Add(mod);

            string version = ItemRandomizerUtil.GetModVersion();

            Assert.AreEqual(mod.VersionString, version);
        }
    }
}

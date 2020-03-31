using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessengerRando.Tests
{
    [TestClass]
    public class RandomizerGenerationTests
    {
        [TestMethod]
        public void TestItemRandomizerUtilLoad()
        {
            //Initilize the util class.
            ItemRandomizerUtil.Load();
            //Attempt to get a mapping
            Dictionary<EItems, EItems> mapping = ItemRandomizerUtil.GenerateRandomizedMappings();
            //Currently this test just makes sure that the Generate makes it all the way through the process and gives us back a mapping.
            Assert.IsNotNull(mapping);
        }
    }
}

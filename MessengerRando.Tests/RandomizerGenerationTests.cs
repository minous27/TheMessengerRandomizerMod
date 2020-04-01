using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessengerRando.Tests
{
    [TestClass]
    public class RandomizerGenerationTests
    {
        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            //Initilize the util class.
            ItemRandomizerUtil.Load();
        }

        [TestMethod]
        public void TestItemRandomizerUtilLoad()
        {
            //Attempt to get a mapping
            Dictionary<EItems, EItems> mapping = ItemRandomizerUtil.GenerateRandomizedMappings();
            //Currently this test just makes sure that the Generate makes it all the way through the process and gives us back a mapping.
            Assert.IsNotNull(mapping);
        }
        
        [TestMethod]
        public void TestGenerateMappingWithSeed()
        {
            int seed = ItemRandomizerUtil.GenerateSeed();

            //We want to generate a mapping with the same seed and get back the same mapping
            Dictionary<EItems, EItems> mapping1 = ItemRandomizerUtil.GenerateRandomizedMappings(seed);
            
            //Logging in case something breaks...
            Console.WriteLine("---mapping1---");
            foreach(EItems item in mapping1.Keys)
            {
                Console.WriteLine(mapping1[item]);
            }
            Dictionary<EItems, EItems> mapping2 = ItemRandomizerUtil.GenerateRandomizedMappings(seed);
            
            Console.WriteLine("---mapping2---");
            foreach (EItems item in mapping2.Keys)
            {
                Console.WriteLine(mapping2[item]);
            }

            //Check to see if the 2 mappings are equal (both keys and values are equal).
            Assert.IsTrue(mapping1.Count == mapping2.Count && !mapping1.Except(mapping2).Any());
        }
        
        [TestMethod]
        public void TestGenerateMappingWithoutSeed()
        {

            //We want to generate a mapping without seeds and get back the different mappings
            Dictionary<EItems, EItems> mapping1 = ItemRandomizerUtil.GenerateRandomizedMappings();

            //Logging in case something breaks...
            Console.WriteLine("---mapping1---");
            foreach (EItems item in mapping1.Keys)
            {
                Console.WriteLine(mapping1[item]);
            }
            Dictionary<EItems, EItems> mapping2 = ItemRandomizerUtil.GenerateRandomizedMappings();

            Console.WriteLine("---mapping2---");
            foreach (EItems item in mapping2.Keys)
            {
                Console.WriteLine(mapping2[item]);
            }

            //Check to see if the 2 mappings are equal (both keys and values are equal).
            Assert.IsFalse(mapping1.Count == mapping2.Count && !mapping1.Except(mapping2).Any());
        }

    }
}

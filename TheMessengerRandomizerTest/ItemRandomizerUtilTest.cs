using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using MessengerRando.Utils;
using MessengerRando.RO;
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

        [TestMethod]
        public void TestGenerateSeed100Times()
        {
            //I want to call GenerateSeed 100 times and fail this test if the number of duplicates I receive exceeds a threshold(5)
            List<int> seeds = new List<int>();
            int threshold = 5;
            int duplicatesFound = 0;
            

            for(int i = 0; i < 100; i++)
            {
                //Generate seed
                int seed = ItemRandomizerUtil.GenerateSeed();

                //Check to see if the list contains the generated seed
                if (!seeds.Contains(seed))
                {
                    seeds.Add(seed);
                }
                else
                {
                    Console.WriteLine($"Duplicate seed generated: {seed}");
                    duplicatesFound++;
                }
                //If we hit the threshold, fail the test
                if (duplicatesFound >= threshold)
                {
                    Assert.Fail("Threshold exceeded.");
                }
            }
            
        }

        [TestMethod]
        public void TestGenerateRandomizedMappingsNoSeed()
        {
            SeedRO seedRO = new SeedRO();
            //Call for random mappings with this empty seed
            Dictionary<LocationRO, RandoItemRO> mappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Assuming we get this far and no issues occur during the mappings, make sure a mapping was returned.
            Assert.IsNotNull(mappings);
        }

        
        [TestMethod]
        public void TestGenerateRandomizedMappingsPredeterminedSeedBasicNoLogic()
        {

            //Set up settings of seed
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            settings.Add(SettingType.Difficulty, SettingValue.Basic);

            //Get a seed for the test
            int seed = ItemRandomizerUtil.GenerateSeed();

            //Set up seed object
            SeedRO seedRO = new SeedRO(SeedType.No_Logic, seed, settings, null);

            //Generate mappings from seed
            Dictionary<LocationRO, RandoItemRO> initialMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);
            Dictionary<LocationRO, RandoItemRO> secondPassMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Compare maps
            foreach(KeyValuePair < LocationRO, RandoItemRO > mapping in initialMappings)
            {
                
                if (!secondPassMappings[mapping.Key].Equals(initialMappings[mapping.Key]))
                {
                    Console.WriteLine($"Initial Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Console.WriteLine($"Second Pass Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Assert.Fail("Mappings were not equal.");
                }
            }
            //Success
        }

        [TestMethod]
        public void TestGenerateRandomizedMappingsPredeterminedSeedBasicLogic()
        {

            //Set up settings of seed
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            settings.Add(SettingType.Difficulty, SettingValue.Basic);

            //Get a seed for the test
            int seed = ItemRandomizerUtil.GenerateSeed();

            //Set up seed object
            SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

            //Generate mappings from seed
            Dictionary<LocationRO, RandoItemRO> initialMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);
            Dictionary<LocationRO, RandoItemRO> secondPassMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Compare maps
            foreach (KeyValuePair<LocationRO, RandoItemRO> mapping in initialMappings)
            {

                if (!secondPassMappings[mapping.Key].Equals(initialMappings[mapping.Key]))
                {
                    Console.WriteLine($"Initial Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Console.WriteLine($"Second Pass Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Assert.Fail("Mappings were not equal.");
                }
            }
            //Success
        }

        [TestMethod]
        public void TestGenerateRandomizedMappingsPredeterminedSeedAdvancedLogic()
        {

            //Set up settings of seed
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            settings.Add(SettingType.Difficulty, SettingValue.Advanced);

            //Get a seed for the test
            int seed = ItemRandomizerUtil.GenerateSeed();

            //Set up seed object
            SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

            //Generate mappings from seed
            Dictionary<LocationRO, RandoItemRO> initialMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);
            Dictionary<LocationRO, RandoItemRO> secondPassMappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Compare maps
            foreach (KeyValuePair<LocationRO, RandoItemRO> mapping in initialMappings)
            {

                if (!secondPassMappings[mapping.Key].Equals(initialMappings[mapping.Key]))
                {
                    Console.WriteLine($"Initial Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Console.WriteLine($"Second Pass Mapping -- Location '{mapping.Key.LocationName}' || Item '{mapping.Value}'");
                    Assert.Fail("Mappings were not equal.");
                }
            }
            //Success
        }




    }
}

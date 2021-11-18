using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using MessengerRando.Utils;
using MessengerRando.RO;
using MessengerRando.Exceptions;
using Mod.Courier;
using Mod.Courier.Module;
using System.Threading;

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
        public void TestGenerateSeedMultipleTimes()
        {
            //I want to call GenerateSeed 10 times and fail this test if the number of duplicates I receive exceeds a threshold(1)
            List<int> seeds = new List<int>();
            int threshold = 1;
            int duplicatesFound = 0;
            

            for(int i = 0; i < 10; i++)
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
                //Let's give it a small wait before trying again
                Thread.Sleep(10);
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

        [TestMethod]
        public void TestIsSeedBeatableGoodSeed()
        {
            //Setup
            EItems[] noAdditionalRequirements = {EItems.NONE};
            EItems[] additionalRequirements = { EItems.DEMON_KING_CROWN, EItems.FAIRY_BOTTLE };

            //Create location with no requirments
            LocationRO locationNoRequirements = new LocationRO("No requirements location", "No requirements location", noAdditionalRequirements, false, false, false);

            //Create location with Ropedart requirements
            LocationRO locationRopedartRequirements = new LocationRO("Ropedart location", "Ropedart location", noAdditionalRequirements, false, true, false);

            //Create location with Wingsuit requirements
            LocationRO locationWingsuitRequirements = new LocationRO("Wingsuit location", "Wingsuit location", noAdditionalRequirements, true, false, false);

            //Create location with Ninja Tabi requirements
            LocationRO locationTabiRequirements = new LocationRO("Ninja Tabi location", "Ninja Tabi location", noAdditionalRequirements, false, false, true);

            //Create location with either ropedart or wingsuit requirements
            LocationRO locationRopedartOrWingsuitRequirements = new LocationRO("Ropedart or Wingsuit location", "Ropedart or Wingsuit location", noAdditionalRequirements, false, false, false, true);

            //Create location with ropedart and wingsuit requirements
            LocationRO locationRopedartAndWingsuitRequirements = new LocationRO("Ropedart and Wingsuit location", "Ropedart and Wingsuit location", noAdditionalRequirements, true, true, false);

            //Create location with all key requirements
            LocationRO locationAllKeyRequirements = new LocationRO("All key location", "All key location", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements2 = new LocationRO("All key location2", "All key location2", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements3 = new LocationRO("All key location3", "All key location3", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements4 = new LocationRO("All key location4", "All key location4", noAdditionalRequirements, true, true, true);

            //Create location with only some additional requirements
            LocationRO locationAdditionalRequirements = new LocationRO("Additional location", "Additional location", additionalRequirements, false, false, false);

            //Create location with a key requirement and some additional requirements
            LocationRO locationKeyAndAdditionalRequirements = new LocationRO("Key and Additional location", "Key and Additional location", additionalRequirements, true, false, false);

            //Load up the mappings
            Dictionary<LocationRO, RandoItemRO> mappings = new Dictionary<LocationRO, RandoItemRO>();
            mappings.Add(locationNoRequirements, new RandoItemRO("Wingsuit", EItems.WINGSUIT));
            mappings.Add(locationRopedartRequirements, new RandoItemRO("Seashell", EItems.SEASHELL));
            mappings.Add(locationWingsuitRequirements, new RandoItemRO("Rope Dart", EItems.GRAPLOU));
            mappings.Add(locationRopedartOrWingsuitRequirements, new RandoItemRO("Ninja Tabi", EItems.MAGIC_BOOTS));
            mappings.Add(locationRopedartAndWingsuitRequirements, new RandoItemRO("Demon Crown", EItems.DEMON_KING_CROWN));
            mappings.Add(locationAllKeyRequirements, new RandoItemRO("Fairy", EItems.FAIRY_BOTTLE));
            mappings.Add(locationAdditionalRequirements, new RandoItemRO("KEY_OF_CHAOS", EItems.KEY_OF_CHAOS));
            mappings.Add(locationKeyAndAdditionalRequirements, new RandoItemRO("KEY_OF_COURAGE", EItems.KEY_OF_COURAGE));
            mappings.Add(locationTabiRequirements, new RandoItemRO("KEY_OF_HOPE", EItems.KEY_OF_HOPE));
            mappings.Add(locationAllKeyRequirements2, new RandoItemRO("KEY_OF_LOVE", EItems.KEY_OF_LOVE));
            mappings.Add(locationAllKeyRequirements3, new RandoItemRO("KEY_OF_STRENGTH", EItems.KEY_OF_STRENGTH));
            mappings.Add(locationAllKeyRequirements4, new RandoItemRO("KEY_OF_SYMBIOSIS", EItems.KEY_OF_SYMBIOSIS));

            //Validate that this now generated seed is beatable
            Assert.IsTrue(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }

        [TestMethod]
        public void TestIsSeedBeatableBadSeed()
        {
            //Setup
            EItems[] noAdditionalRequirements = { EItems.NONE };
            EItems[] additionalRequirements = { EItems.DEMON_KING_CROWN, EItems.FAIRY_BOTTLE };

            //Create location with no requirments
            LocationRO locationNoRequirements = new LocationRO("No requirements location", "No requirements location", noAdditionalRequirements, false, false, false);

            //Create location with Ropedart requirements
            LocationRO locationRopedartRequirements = new LocationRO("Ropedart location", "Ropedart location", noAdditionalRequirements, false, true, false);

            //Create location with Wingsuit requirements
            LocationRO locationWingsuitRequirements = new LocationRO("Wingsuit location", "Wingsuit location", noAdditionalRequirements, true, false, false);

            //Create location with Ninja Tabi requirements
            LocationRO locationTabiRequirements = new LocationRO("Ninja Tabi location", "Ninja Tabi location", noAdditionalRequirements, false, false, true);

            //Create location with either ropedart or wingsuit requirements
            LocationRO locationRopedartOrWingsuitRequirements = new LocationRO("Ropedart or Wingsuit location", "Ropedart or Wingsuit location", noAdditionalRequirements, false, false, false, true);

            //Create location with ropedart and wingsuit requirements
            LocationRO locationRopedartAndWingsuitRequirements = new LocationRO("Ropedart and Wingsuit location", "Ropedart and Wingsuit location", noAdditionalRequirements, true, true, false);

            //Create location with all key requirements
            LocationRO locationAllKeyRequirements = new LocationRO("All key location", "All key location", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements2 = new LocationRO("All key location2", "All key location2", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements3 = new LocationRO("All key location3", "All key location3", noAdditionalRequirements, true, true, true);
            LocationRO locationAllKeyRequirements4 = new LocationRO("All key location4", "All key location4", noAdditionalRequirements, true, true, true);

            //Create location with only some additional requirements
            LocationRO locationAdditionalRequirements = new LocationRO("Additional location", "Additional location", additionalRequirements, false, false, false);

            //Create location with a key requirement and some additional requirements
            LocationRO locationKeyAndAdditionalRequirements = new LocationRO("Key and Additional location", "Key and Additional location", additionalRequirements, true, false, false);

            //Load up the mappings
            Dictionary<LocationRO, RandoItemRO> mappings = new Dictionary<LocationRO, RandoItemRO>();
            mappings.Add(locationRopedartRequirements, new RandoItemRO("Seashell", EItems.SEASHELL));
            mappings.Add(locationWingsuitRequirements, new RandoItemRO("Rope Dart", EItems.GRAPLOU));
            mappings.Add(locationRopedartOrWingsuitRequirements, new RandoItemRO("Ninja Tabi", EItems.MAGIC_BOOTS));
            mappings.Add(locationRopedartAndWingsuitRequirements, new RandoItemRO("Demon Crown", EItems.DEMON_KING_CROWN));
            mappings.Add(locationAllKeyRequirements, new RandoItemRO("Fairy", EItems.FAIRY_BOTTLE));
            mappings.Add(locationAdditionalRequirements, new RandoItemRO("KEY_OF_CHAOS", EItems.KEY_OF_CHAOS));
            mappings.Add(locationKeyAndAdditionalRequirements, new RandoItemRO("KEY_OF_COURAGE", EItems.KEY_OF_COURAGE));
            mappings.Add(locationTabiRequirements, new RandoItemRO("KEY_OF_HOPE", EItems.KEY_OF_HOPE));
            mappings.Add(locationAllKeyRequirements2, new RandoItemRO("KEY_OF_LOVE", EItems.KEY_OF_LOVE));
            mappings.Add(locationAllKeyRequirements3, new RandoItemRO("KEY_OF_STRENGTH", EItems.KEY_OF_STRENGTH));
            mappings.Add(locationAllKeyRequirements4, new RandoItemRO("KEY_OF_SYMBIOSIS", EItems.KEY_OF_SYMBIOSIS));

            //Validate that this seed is not beatable
            Assert.IsFalse(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }

        [TestMethod]
        public void TestIsBasicSeedBeatableGeneratedSeed()
        {
            //Set up settings of seed
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            settings.Add(SettingType.Difficulty, SettingValue.Basic);

            //Get a seed for the test
            int seed = ItemRandomizerUtil.GenerateSeed();

            //Set up seed object
            SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

            //Generate mappings
            Dictionary<LocationRO, RandoItemRO> mappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Validate that this now generated seed is beatable
            Assert.IsTrue(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }

        [TestMethod]
        public void TestIsMultipleBasicSeedBeatableGeneratedSeed()
        {
            bool isAllBeatableSeeds = true;
            int iterations = 5;

            //Threshold for acceptable generation errors to prevent an infinite loop.
            int generationFailureThreshold = 5;
            int generationFailureCount = 0;

            for (int i = 0; i < iterations; i++)
            {
                //Set up settings of seed
                Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
                settings.Add(SettingType.Difficulty, SettingValue.Basic);

                //Get a seed for the test
                int seed = ItemRandomizerUtil.GenerateSeed();

                //Set up seed object
                SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

                //Generate mappings
                Dictionary<LocationRO, RandoItemRO> mappings = null;
                try
                {
                    mappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);
                }
                catch(RandomizerException)
                {
                    //This happens when the actual generation can't create a mapping. This is the expected behavior and not what we are testing here.
                    Console.WriteLine("An error occurred during generation. Logging the errored instance and moving on.");
                    if(++generationFailureCount >= generationFailureThreshold)
                    {
                        //This means that generation is failing WAY too much. There must be something wrong and it needs to be looked into.
                        Assert.Fail("Generation failures exceeded allowed threshold. We need to look into why generation is having such a hard time making seeds...");
                    }

                    //Try again
                    --i;
                    continue;
                }
                //Validate that this now generated seed is beatable
                if(!ItemRandomizerUtil.IsSeedBeatable(mappings))
                {
                    isAllBeatableSeeds = false;
                    break;
                }
            }
            Assert.IsTrue(isAllBeatableSeeds);
        }

        [TestMethod]
        public void TestIsAdvancedSeedBeatableGeneratedSeed()
        {
            //Set up settings of seed
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            settings.Add(SettingType.Difficulty, SettingValue.Advanced);

            //Get a seed for the test
            int seed = ItemRandomizerUtil.GenerateSeed();

            //Set up seed object
            SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

            //Generate mappings
            Dictionary<LocationRO, RandoItemRO> mappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

            //Validate that this now generated seed is beatable
            Assert.IsTrue(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }

        [TestMethod]
        public void TestIsMultipleAdvancedSeedBeatableGeneratedSeed()
        {
            bool isAllBeatableSeeds = true;
            int iterations = 5;

            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine($"Running test on generation number {i + 1}");

                //Set up settings of seed
                Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
                settings.Add(SettingType.Difficulty, SettingValue.Advanced);

                //Get a seed for the test
                int seed = ItemRandomizerUtil.GenerateSeed();

                //Set up seed object
                SeedRO seedRO = new SeedRO(SeedType.Logic, seed, settings, null);

                //Generate mappings
                Dictionary<LocationRO, RandoItemRO> mappings = ItemRandomizerUtil.GenerateRandomizedMappings(seedRO);

                //Validate that this now generated seed is beatable
                if (!ItemRandomizerUtil.IsSeedBeatable(mappings))
                {
                    isAllBeatableSeeds = false;
                    break;
                }
            }
            Assert.IsTrue(isAllBeatableSeeds);
        }




    }
}

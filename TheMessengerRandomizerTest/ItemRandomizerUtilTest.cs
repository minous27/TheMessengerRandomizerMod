using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using MessengerRando.Utils;
using MessengerRando.RO;
using MessengerRando.Exceptions;
using Mod.Courier;
using Mod.Courier.Module;
using System.Threading;
using System.Text.Json;
using UnityEngine;


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
        public void TestIsSeedBeatablePredeterminedGoodSeed()
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

            //Logging
            Console.WriteLine($"\nStatic good seed. Mappings are as follows:\n");

            foreach (LocationRO location in mappings.Keys)
            {
                Console.WriteLine($"Location '{location.PrettyLocationName}' contains item '{mappings[location]}'");
            }

            //Validate that this now generated seed is beatable
            Assert.IsTrue(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }

        [TestMethod]
        public void TestIsSeedBeatablePredeterminedBadSeed()
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

            //Logging
            Console.WriteLine($"\nStatic Bad Seed. Mappings are as follows:\n");

            foreach (LocationRO location in mappings.Keys)
            {
                Console.WriteLine($"Location '{location.PrettyLocationName}' contains item '{mappings[location]}'");
            }

            //Validate that this seed is not beatable
            Assert.IsFalse(ItemRandomizerUtil.IsSeedBeatable(mappings));

        }
    }
}

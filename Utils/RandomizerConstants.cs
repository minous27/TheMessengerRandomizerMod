using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerRando.RO;

namespace MessengerRando.Utils
{
    class RandomizerConstants
    {
        public static readonly EItems[] randomizedItems = new EItems[] {EItems.WINDMILL_SHURIKEN, EItems.WINGSUIT, EItems.GRAPLOU,
            EItems.MAGIC_BOOTS, EItems.CANDLE, EItems.SEASHELL, EItems.POWER_THISTLE, EItems.DEMON_KING_CROWN, EItems.RUXXTIN_AMULET,
            EItems.FAIRY_BOTTLE, EItems.SUN_CREST, EItems.MOON_CREST, EItems.NECROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER};

        public static readonly EItems[] notes = new EItems[] { EItems.KEY_OF_HOPE, EItems.KEY_OF_CHAOS, EItems.KEY_OF_COURAGE, EItems.KEY_OF_LOVE, EItems.KEY_OF_STRENGTH, EItems.KEY_OF_SYMBIOSIS };



        public static Dictionary<EItems,string> GetDialogIDtoItems()
        {
            Dictionary<EItems, string> itemDialogID = new Dictionary<EItems, string>();

            itemDialogID.Add(EItems.CLIMBING_CLAWS, "AWARD_GRIMPLOU");
            itemDialogID.Add(EItems.WINGSUIT, "AWARD_WINGSUIT");
            itemDialogID.Add(EItems.GRAPLOU, "AWARD_ROPE_DART");
            itemDialogID.Add(EItems.FAIRY_BOTTLE, "AWARD_FAIRY");
            itemDialogID.Add(EItems.MAGIC_BOOTS, "AWARD_MAGIC_BOOTS");
            itemDialogID.Add(EItems.SEASHELL, "AWARD_MAGIC_SEASHELL");
            itemDialogID.Add(EItems.RUXXTIN_AMULET, "AWARD_AMULET");
            //itemDialogID.Add(EItems.TEA_SEED, "AWARD_SEED");
            //itemDialogID.Add(EItems.TEA_LEAVES, "AWARD_ASTRAL_LEAVES");
            itemDialogID.Add(EItems.POWER_THISTLE, "AWARD_THISTLE");
            itemDialogID.Add(EItems.CANDLE, "AWARD_CANDLE");
            itemDialogID.Add(EItems.DEMON_KING_CROWN, "AWARD_CROWN");
            itemDialogID.Add(EItems.WINDMILL_SHURIKEN, "AWARD_WINDMILL");
            itemDialogID.Add(EItems.KEY_OF_HOPE, "AWARD_KEY_OF_HOPE");
            itemDialogID.Add(EItems.KEY_OF_STRENGTH, "AWARD_KEY_OF_STRENGTH");
            itemDialogID.Add(EItems.KEY_OF_CHAOS, "AWARD_KEY_OF_CHAOS");
            itemDialogID.Add(EItems.KEY_OF_LOVE, "AWARD_KEY_OF_LOVE");
            itemDialogID.Add(EItems.KEY_OF_SYMBIOSIS, "AWARD_KEY_OF_SYMBIOSIS");
            itemDialogID.Add(EItems.KEY_OF_COURAGE, "AWARD_KEY_OF_COURAGE");
            itemDialogID.Add(EItems.SUN_CREST, "AWARD_SUN_CREST");
            itemDialogID.Add(EItems.MOON_CREST, "AWARD_MOON_CREST");

            itemDialogID.Add(EItems.PYROPHOBIC_WORKER, "FIND_PYRO");
            itemDialogID.Add(EItems.ACROPHOBIC_WORKER, "FIND_ACRO");
            itemDialogID.Add(EItems.NECROPHOBIC_WORKER, "NECRO_PHOBEKIN_DIALOG");
            itemDialogID.Add(EItems.CLAUSTROPHOBIC_WORKER, "FIND_CLAUSTRO");

            return itemDialogID;

        }



        public static List<LocationRO> GetRandoLocationList()
        {
            //Create a LocationRO for every check and put it in a list
            List<LocationRO> randomizedLocations = new List<LocationRO>();
            //Phase 1 section, no key items required
            randomizedLocations.Add(new LocationRO(EItems.SEASHELL, new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.MAGIC_BOOTS, new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.GRAPLOU, new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.WINGSUIT, new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_LOVE, new EItems[] { EItems.SUN_CREST, EItems.MOON_CREST }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_COURAGE, new EItems[] { EItems.DEMON_KING_CROWN, EItems.FAIRY_BOTTLE }, false, false, false));
            //Tabi locked section
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_CHAOS, new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.SUN_CREST, new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.MOON_CREST, new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.PYROPHOBIC_WORKER, new EItems[] { EItems.NONE }, false, false, true));
            //Wingsuit locked section
            randomizedLocations.Add(new LocationRO(EItems.ACROPHOBIC_WORKER, new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.NECROPHOBIC_WORKER, new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.RUXXTIN_AMULET, new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CANDLE, new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CLAUSTROPHOBIC_WORKER, new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CLIMBING_CLAWS, new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.DEMON_KING_CROWN, new EItems[] { EItems.NECROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER }, true, false, false, false));
            //Rope Dart locked section
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_SYMBIOSIS, new EItems[] { EItems.FAIRY_BOTTLE }, false, true, false));
            //This section needs either the Wingsuit OR the Ropedart (plus other items). If you have one you are good.
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_STRENGTH, new EItems[] { EItems.POWER_THISTLE }, false, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.POWER_THISTLE, new EItems[] { EItems.NONE }, false, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.FAIRY_BOTTLE, new EItems[] { EItems.NONE }, false, false, false, true));
            //Wingsuit AND Ropedart locked
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_HOPE, new EItems[] { EItems.NONE }, true, true, false));

            return randomizedLocations;
        }

        public static List<string> GetSpecialTriggerNames()
        {
            List<string> triggersToIgnoreRandoItemLogic = new List<string>();

            //LOAD (initally started as a black list of locations...probably would have been better to make this a whitelist...whatever)
            triggersToIgnoreRandoItemLogic.Add("CorruptedFuturePortal"); //Need to really check for crown and get access to CF
            triggersToIgnoreRandoItemLogic.Add("Lucioles"); //CF Fairy Check
            triggersToIgnoreRandoItemLogic.Add("DecurseQueenCutscene");
            triggersToIgnoreRandoItemLogic.Add("Bridge"); //Forlorn bridge check
            triggersToIgnoreRandoItemLogic.Add("NoUpgrade"); //Dark Cave Candle check
            triggersToIgnoreRandoItemLogic.Add("OverlayArt_16"); //...also Dark Cave Candle check
            //These are for the sprite renderings of phoebes
            triggersToIgnoreRandoItemLogic.Add("PhobekinNecro");
            triggersToIgnoreRandoItemLogic.Add("PhobekinNecro_16");
            triggersToIgnoreRandoItemLogic.Add("PhobekinAcro");
            triggersToIgnoreRandoItemLogic.Add("PhobekinAcro_16");
            triggersToIgnoreRandoItemLogic.Add("PhobekinClaustro");
            triggersToIgnoreRandoItemLogic.Add("PhobekinClaustro_16");
            triggersToIgnoreRandoItemLogic.Add("PhobekinPyro");
            triggersToIgnoreRandoItemLogic.Add("PhobekinPyro_16");
            //Parents of triggers to handle sassy interaction zones
            triggersToIgnoreRandoItemLogic.Add("Colos_8");
            triggersToIgnoreRandoItemLogic.Add("Suses_8");
            triggersToIgnoreRandoItemLogic.Add("Door");
            triggersToIgnoreRandoItemLogic.Add("RuxtinStaff");

            return triggersToIgnoreRandoItemLogic;
        }

        public static Dictionary<string, EItems> GetCutsceneMappings()
        {
            //This is where all the cutscene mappings will live. These mappings will mean that the cutscene requires additional logic to ensure it has "been played" or not.
            Dictionary<string, EItems> cutsceneMappings = new Dictionary<string, EItems>();

            //LOAD
            cutsceneMappings.Add("RuxxtinNoteAndAwardAmuletCutscene", EItems.RUXXTIN_AMULET);

            return cutsceneMappings;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerRando.RO;

namespace MessengerRando.Utils
{
    class RandomizerConstants
    {
        /* OLD
        public static readonly EItems[] randomizedItems = new EItems[] {EItems.WINDMILL_SHURIKEN, EItems.WINGSUIT, EItems.GRAPLOU,
            EItems.MAGIC_BOOTS, EItems.CANDLE, EItems.SEASHELL, EItems.POWER_THISTLE, EItems.DEMON_KING_CROWN, EItems.RUXXTIN_AMULET,
            EItems.FAIRY_BOTTLE, EItems.SUN_CREST, EItems.MOON_CREST, EItems.NECROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER};

        public static readonly EItems[] randomizedAdvancedItems = new EItems[] { EItems.FEATHER, EItems.FEATHER, EItems.FEATHER, EItems.FEATHER,
            EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE, EItems.MASK_PIECE,
            EItems.MONEY_WRENCH, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION, EItems.POTION,
            EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD,
            EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD, EItems.TIME_SHARD};

        public static readonly EItems[] notes = new EItems[] { EItems.KEY_OF_HOPE, EItems.KEY_OF_CHAOS, EItems.KEY_OF_COURAGE, EItems.KEY_OF_LOVE, EItems.KEY_OF_STRENGTH, EItems.KEY_OF_SYMBIOSIS };
        */
        public static List<RandoItemRO> GetNotesList()
        {
            List<RandoItemRO> notes = new List<RandoItemRO>();

            notes.Add(new RandoItemRO("Key-Of-Hope", EItems.KEY_OF_HOPE));
            notes.Add(new RandoItemRO("Key-Of-Chaos", EItems.KEY_OF_CHAOS));
            notes.Add(new RandoItemRO("Key-Of-Courage", EItems.KEY_OF_COURAGE));
            notes.Add(new RandoItemRO("Key-Of-Love", EItems.KEY_OF_LOVE));
            notes.Add(new RandoItemRO("Key-Of-Strength", EItems.KEY_OF_STRENGTH));
            notes.Add(new RandoItemRO("Key-Of-Symbiosis", EItems.KEY_OF_SYMBIOSIS));

            return notes;
        }


        public static List<RandoItemRO> GetRandoItemList()
        {
            List<RandoItemRO> randomizedItems = new List<RandoItemRO>();

            randomizedItems.Add(new RandoItemRO("Windmill-Shuriken", EItems.WINDMILL_SHURIKEN));
            randomizedItems.Add(new RandoItemRO("Wingsuit", EItems.WINGSUIT));
            randomizedItems.Add(new RandoItemRO("Rope-Dart", EItems.GRAPLOU));
            randomizedItems.Add(new RandoItemRO("Ninja-Tabis", EItems.MAGIC_BOOTS));
            randomizedItems.Add(new RandoItemRO("Candle", EItems.CANDLE));
            randomizedItems.Add(new RandoItemRO("Seashell", EItems.SEASHELL));
            randomizedItems.Add(new RandoItemRO("Power-Thistle", EItems.POWER_THISTLE));
            randomizedItems.Add(new RandoItemRO("Demon-King-Crown", EItems.DEMON_KING_CROWN));
            randomizedItems.Add(new RandoItemRO("Ruxxtin-Amulet", EItems.RUXXTIN_AMULET));
            randomizedItems.Add(new RandoItemRO("Fairy-Bottle", EItems.FAIRY_BOTTLE));
            randomizedItems.Add(new RandoItemRO("Sun-Crest", EItems.SUN_CREST));
            randomizedItems.Add(new RandoItemRO("Moon-Crest", EItems.MOON_CREST));
            randomizedItems.Add(new RandoItemRO("Necro", EItems.NECROPHOBIC_WORKER));
            randomizedItems.Add(new RandoItemRO("Pyro", EItems.PYROPHOBIC_WORKER));
            randomizedItems.Add(new RandoItemRO("Claustro", EItems.CLAUSTROPHOBIC_WORKER));
            randomizedItems.Add(new RandoItemRO("Acro", EItems.ACROPHOBIC_WORKER));

            return randomizedItems;
        }
        
        public static List<RandoItemRO> GetAdvancedRandoItemList()
        {
            List<RandoItemRO> randomizedItems = new List<RandoItemRO>();

            randomizedItems.Add(new RandoItemRO("Feather-1", EItems.FEATHER));
            randomizedItems.Add(new RandoItemRO("Feather-2", EItems.FEATHER));
            randomizedItems.Add(new RandoItemRO("Feather-3", EItems.FEATHER));
            randomizedItems.Add(new RandoItemRO("Feather-4", EItems.FEATHER));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-1", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-2", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-3", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-4", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-5", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-6", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-7", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-8", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-9", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Mask-Piece-10", EItems.MASK_PIECE));
            randomizedItems.Add(new RandoItemRO("Money-Wrench", EItems.MONEY_WRENCH));
            randomizedItems.Add(new RandoItemRO("Potion-1", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-2", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-3", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-4", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-5", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-6", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-7", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-8", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-9", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Potion-10", EItems.POTION));
            randomizedItems.Add(new RandoItemRO("Timeshard-1", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-2", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-3", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-4", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-5", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-6", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-7", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-8", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-9", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-10", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-11", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-12", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-13", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-14", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-15", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-16", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-17", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-18", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-19", EItems.TIME_SHARD));
            randomizedItems.Add(new RandoItemRO("Timeshard-20", EItems.TIME_SHARD));

            return randomizedItems;
        }


        public static List<LocationRO> GetRandoLocationList()
        {
            //Create a LocationRO for every check and put it in a list
            List<LocationRO> randomizedLocations = new List<LocationRO>();
            //Phase 1 section, no key items required
            randomizedLocations.Add(new LocationRO(EItems.SEASHELL.ToString(), EItems.SEASHELL.ToString(), new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.MAGIC_BOOTS.ToString(), EItems.MAGIC_BOOTS.ToString(), new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.GRAPLOU.ToString(), EItems.GRAPLOU.ToString(), new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.WINGSUIT.ToString(), EItems.WINGSUIT.ToString(), new EItems[] { EItems.NONE }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_LOVE.ToString(), EItems.KEY_OF_LOVE.ToString(), new EItems[] { EItems.SUN_CREST, EItems.MOON_CREST }, false, false, false));
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_COURAGE.ToString(), EItems.KEY_OF_COURAGE.ToString(), new EItems[] { EItems.DEMON_KING_CROWN, EItems.FAIRY_BOTTLE }, false, false, false));
            //Tabi locked section
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_CHAOS.ToString(), EItems.KEY_OF_CHAOS.ToString(), new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.SUN_CREST.ToString(), EItems.SUN_CREST.ToString(), new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.MOON_CREST.ToString(), EItems.MOON_CREST.ToString(), new EItems[] { EItems.NONE }, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.PYROPHOBIC_WORKER.ToString(), EItems.PYROPHOBIC_WORKER.ToString(), new EItems[] { EItems.NONE }, false, false, true));
            //Wingsuit locked section
            randomizedLocations.Add(new LocationRO(EItems.ACROPHOBIC_WORKER.ToString(), EItems.ACROPHOBIC_WORKER.ToString(), new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.NECROPHOBIC_WORKER.ToString(), EItems.NECROPHOBIC_WORKER.ToString(), new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.RUXXTIN_AMULET.ToString(), EItems.RUXXTIN_AMULET.ToString(), new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CANDLE.ToString(), EItems.CANDLE.ToString(), new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CLAUSTROPHOBIC_WORKER.ToString(), EItems.CLAUSTROPHOBIC_WORKER.ToString(), new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.CLIMBING_CLAWS.ToString(), EItems.CLIMBING_CLAWS.ToString(), new EItems[] { EItems.NONE }, true, false, false));
            randomizedLocations.Add(new LocationRO(EItems.DEMON_KING_CROWN.ToString(), EItems.DEMON_KING_CROWN.ToString(), new EItems[] { EItems.NECROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER }, true, false, false, false));
            //Rope Dart locked section
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_SYMBIOSIS.ToString(), EItems.KEY_OF_SYMBIOSIS.ToString(), new EItems[] { EItems.FAIRY_BOTTLE }, false, true, false));
            //This section needs either the Wingsuit OR the Ropedart (plus other items). If you have one you are good.
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_STRENGTH.ToString(), EItems.KEY_OF_STRENGTH.ToString(), new EItems[] { EItems.POWER_THISTLE }, false, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.POWER_THISTLE.ToString(), EItems.POWER_THISTLE.ToString(), new EItems[] { EItems.NONE }, false, false, false, true));
            randomizedLocations.Add(new LocationRO(EItems.FAIRY_BOTTLE.ToString(), EItems.FAIRY_BOTTLE.ToString(), new EItems[] { EItems.NONE }, false, false, false, true));
            //Wingsuit AND Ropedart locked
            randomizedLocations.Add(new LocationRO(EItems.KEY_OF_HOPE.ToString(), EItems.KEY_OF_HOPE.ToString(), new EItems[] { EItems.NONE }, true, true, false));

            return randomizedLocations;
        }

        public static List<LocationRO> GetAdvancedRandoLocationList()
        {
            //Create a LocationRO for every check and put it in a list
            List<LocationRO> advancedRandomizedLocations = new List<LocationRO>();

            //Adding seal locations

            //Ninja Village
            advancedRandomizedLocations.Add(new LocationRO("-436-404-44-28", "Ninja Village Seal - Tree House", new EItems[] { EItems.NONE }, true, true, false)); //Tree House
            //Autumn Hills
            advancedRandomizedLocations.Add(new LocationRO("-52-20-60-44", "Autumn Hills Seal - Trip Saws", new EItems[] { EItems.NONE }, true, false, false)); //Trip Saws
            advancedRandomizedLocations.Add(new LocationRO("556588-44-28", "Autumn Hills Seal - Double Swing Saws", new EItems[] { EItems.NONE }, true, false, false)); //Double Swing Saws
            advancedRandomizedLocations.Add(new LocationRO("748780-76-60", "Autumn Hills Seal - Spike Ball Swing", new EItems[] { EItems.NONE }, true, false, false)); //Spike Ball Swing
            advancedRandomizedLocations.Add(new LocationRO("748780-108-76", "Autumn Hills Seal - Spike Ball Darts", new EItems[] { EItems.NONE }, true, false, false)); //Spike Ball Darts - also requires Acrobatic Warrior upgrade
            //Catacombs
            advancedRandomizedLocations.Add(new LocationRO("236268-44-28", "Catacombs Seal - Triple Spike Crushers", new EItems[] { EItems.NONE }, true, false, false)); //Triple Spike Crushers
            advancedRandomizedLocations.Add(new LocationRO("492524-44-28", "Catacombs Seal - Crusher Gauntlet", new EItems[] { EItems.NONE }, true, false, false)); //Crusher Gauntlet
            advancedRandomizedLocations.Add(new LocationRO("556588-60-44", "Catacombs Seal - Dirty Pond", new EItems[] { EItems.NONE }, true, false, false)); //Dirty Pond
            //Bamboo Creek
            advancedRandomizedLocations.Add(new LocationRO("-84-52-28-12", "Bamboo Creek Seal - Spike crushers and Doors", new EItems[] { EItems.NONE }, true, false, false)); //Spike crushers and Doors
            advancedRandomizedLocations.Add(new LocationRO("172236-44-28", "Bamboo Creek Seal - Spike ball pits", new EItems[] { EItems.NONE }, true, false, false)); //Spike ball pits
            advancedRandomizedLocations.Add(new LocationRO("300332-1236", "Bamboo Creek Seal - Spike crushers and Doors v2", new EItems[] { EItems.NONE }, true, false, false)); //Spike crushers and doors v2
            //Howling Grotto
            advancedRandomizedLocations.Add(new LocationRO("108140-28-12", "Howling Grotto Seal - Windy Saws and Balls", new EItems[] { EItems.NONE }, false, false, false)); //Windy Saws and Balls
            advancedRandomizedLocations.Add(new LocationRO("300332-92-76", "Howling Grotto Seal - Crushing Pits", new EItems[] { EItems.NONE }, false, false, false)); //Crushing Pits
            advancedRandomizedLocations.Add(new LocationRO("460492-172-156", "Howling Grotto Seal - Breezy Crushers", new EItems[] { EItems.NONE }, false, false, false)); //Breezy Crushers
            //Quillshroom Marsh
            advancedRandomizedLocations.Add(new LocationRO("204236-28-12", "Quillshroom Marsh Seal - Spikey Window", new EItems[] { EItems.NONE }, false, false, false)); //Spikey Window
            advancedRandomizedLocations.Add(new LocationRO("652684-60-28", "Quillshroom Marsh Seal - Sand Trap", new EItems[] { EItems.NONE }, false, false, false)); //Sand Trap
            advancedRandomizedLocations.Add(new LocationRO("940972-28-12", "Quillshroom Marsh Seal - Do the Spike Wave", new EItems[] { EItems.NONE }, false, false, false)); //Do the Spike Wave
            //Searing Crags
            advancedRandomizedLocations.Add(new LocationRO("761085268", "Searing Crags Seal - Triple Ball Spinner", new EItems[] { EItems.NONE }, false, false, false, true)); //Triple Ball Spinner
            advancedRandomizedLocations.Add(new LocationRO("300332196212", "Searing Crags Seal - Raining Rocks", new EItems[] { EItems.NONE }, false, false, false, true)); //Raining Rocks
            advancedRandomizedLocations.Add(new LocationRO("364396292308", "Searing Crags Seal - Rythym Rocks", new EItems[] { EItems.NONE }, false, false, false, true)); //Rythym Rocks
            //Glacial Peak
            advancedRandomizedLocations.Add(new LocationRO("140172-492-476", "Glacial Peak Seal - Ice Climbers", new EItems[] { EItems.NONE }, false, true, false)); //Ice Climbers
            advancedRandomizedLocations.Add(new LocationRO("236268-396-380", "Glacial Peak Seal - Projectile Spike Pit", new EItems[] { EItems.NONE }, false, false, false, true)); //Projectile Spike Pit
            advancedRandomizedLocations.Add(new LocationRO("236268-156-140", "Glacial Peak Seal - Glacial Air Swag", new EItems[] { EItems.NONE }, false, false, false, true)); //Glacial Air Swag
            //TowerOfTime
            advancedRandomizedLocations.Add(new LocationRO("-84-522036", "TowerOfTime Seal - Time Waster Seal", new EItems[] { EItems.NONE }, false, true, false)); //Time Waster Seal
            advancedRandomizedLocations.Add(new LocationRO("7610852116", "TowerOfTime Seal - Lantern Climb", new EItems[] { EItems.NONE }, true, false, false)); //Lantern Climb
            advancedRandomizedLocations.Add(new LocationRO("-52-20116132", "TowerOfTime Seal - Arcane Orbs", new EItems[] { EItems.NONE }, true, true, false)); //Arcane Orbs
            //Cloud Ruins
            advancedRandomizedLocations.Add(new LocationRO("-148-116420", "Cloud Ruins Seal - Ghost Pit", new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false)); //Ghost Pit
            advancedRandomizedLocations.Add(new LocationRO("108140-44-28", "Cloud Ruins Seal - Toothbrush Alley", new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false)); //Toothbrush Alley
            advancedRandomizedLocations.Add(new LocationRO("748780-44-28", "Cloud Ruins Seal - Saw Pit", new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false)); //Saw Pit
            advancedRandomizedLocations.Add(new LocationRO("11321164-124", "Cloud Ruins Seal - Money Farm Room", new EItems[] { EItems.RUXXTIN_AMULET }, true, false, false)); //Money Farm Room
            //Underworld
            advancedRandomizedLocations.Add(new LocationRO("-276-244-444", "Underworld Seal - Sharp and Windy Climb", new EItems[] { EItems.NONE }, true, false, true)); //Sharp and Windy Climb
            advancedRandomizedLocations.Add(new LocationRO("-180-148-44-28", "Underworld Seal - Spike Wall", new EItems[] { EItems.NONE }, false, false, true)); //Spike Wall
            advancedRandomizedLocations.Add(new LocationRO("-180-148-60-44", "Underworld Seal - Fireball Wave", new EItems[] { EItems.NONE }, true, false, true)); //Fireball Wave - also requires Acrobatic Warrior upgrade
            advancedRandomizedLocations.Add(new LocationRO("-2012-124", "Underworld Seal - Rising Fanta", new EItems[] { EItems.NONE }, false, true, true)); //Rising Fanta
            //Forlorn Temple
            advancedRandomizedLocations.Add(new LocationRO("172268-284", "Forlorn Temple Seal - Rocket Maze", new EItems[] { EItems.NECROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER }, true, false, false)); //Rocket Maze
            advancedRandomizedLocations.Add(new LocationRO("140172100164", "Forlorn Temple Seal - Rocket Sunset", new EItems[] { EItems.NECROPHOBIC_WORKER, EItems.CLAUSTROPHOBIC_WORKER, EItems.PYROPHOBIC_WORKER, EItems.ACROPHOBIC_WORKER }, true, false, false)); //Rocket Sunset
            //Sunken Shrine
            advancedRandomizedLocations.Add(new LocationRO("204236-124", "Sunken Shrine Seal - Ultra Lifeguard", new EItems[] { EItems.NONE }, false, false, false)); //Ultra Lifeguard
            advancedRandomizedLocations.Add(new LocationRO("172268-188-172", "Sunken Shrine Seal - Waterfall Paradise", new EItems[] { EItems.NONE }, false, false, true)); //Waterfall Paradise
            advancedRandomizedLocations.Add(new LocationRO("-148-116-124-60", "Sunken Shrine Seal - Tabi Gauntlet", new EItems[] { EItems.NONE }, false, false, true)); //Tabi Gauntlet
            //Reviere Turquoise
            advancedRandomizedLocations.Add(new LocationRO("844876-284", "Reviere Turquoise Seal - Bounces and Balls", new EItems[] { EItems.NONE }, false, false, false)); //Bounces and Balls
            advancedRandomizedLocations.Add(new LocationRO("460492-124-108", "Reviere Turquoise Seal - Launch of Faith", new EItems[] { EItems.NONE }, false, false, false)); //Launch of Faith
            advancedRandomizedLocations.Add(new LocationRO("-180-1483668", "Reviere Turquoise Seal - Flower Power", new EItems[] { EItems.NONE }, false, false, false, true)); //Flower Power
            //Elemental Skylands
            advancedRandomizedLocations.Add(new LocationRO("-52-20420436", "Elemental Skylands Seal - Air Seal", new EItems[] { EItems.FAIRY_BOTTLE }, true, false, false)); //Air Seal
            advancedRandomizedLocations.Add(new LocationRO("18361868372388", "Elemental Skylands Seal - Water Seal", new EItems[] { EItems.FAIRY_BOTTLE }, false, true, false)); //Water Seal - Needs water dash
            advancedRandomizedLocations.Add(new LocationRO("28602892356388", "Elemental Skylands Seal - Fire Seal", new EItems[] { EItems.FAIRY_BOTTLE }, true, true, false)); //Fire Seal


            return advancedRandomizedLocations;
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

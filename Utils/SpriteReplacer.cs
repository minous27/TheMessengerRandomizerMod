using MessengerRando.Exceptions;
using MessengerRando.RO;
using Mod.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

//Replacements to implement \ work out
//  - Magic Firefly - Needs a condition to keep original sprite until after fight.
//      Currently always replaces, bug or feature?



namespace MessengerRando.Utils
{
    /// <summary>
    /// Class for replacing item sprites using their inventory icon
    /// Actually just hides the real sprites and adds a new child component with a sprite renderer
    /// Future: Do proper replacements
    /// </summary>
    public static class SpriteReplacer
    {
        /// <summary>
        /// Determines which level is loaded and what sprites need replacing for each level
        /// </summary>
        /// <param name="LI">Level Initializer</param>
        static void LevelInitializer_Rando(LevelInitializer LI)
        {
       
            if (LevelManager.Instance != null)
            {
                //Which level are we loading the replacements
                switch (LevelManager.Instance.GetCurrentLevelEnum())
                {
                    case ELevel.Level_02_AutumnHills:
                        ReplaceNotes(EItems.KEY_OF_HOPE);
                        break;
                    case ELevel.Level_03_ForlornTemple:
                        ReplaceDemonCrown();
                        break;
                    case ELevel.Level_04_Catacombs:
                        ReplacePhobekin(EItems.NECROPHOBIC_WORKER);
                        ReplaceRuxxtinRomb();
                        break;
                    case ELevel.Level_04_C_RiviereTurquoise:
                        ReplaceFirefly();
                        break;
                    case ELevel.Level_05_A_HowlingGrotto:
                        break;
                    case ELevel.Level_05_B_SunkenShrine:
                        ReplaceNotes(EItems.KEY_OF_LOVE);
                        ReplaceInWorldItem(EItems.MOON_CREST);
                        ReplaceInWorldItem(EItems.SUN_CREST);
                        ReplaceInWorldItem(EItems.MAGIC_BOOTS);
                        break;
                    case ELevel.Level_06_A_BambooCreek:
                        ReplacePhobekin(EItems.CLAUSTROPHOBIC_WORKER);
                        break;
                    case ELevel.Level_07_QuillshroomMarsh:
                        ReplaceInWorldItem(EItems.SEASHELL);
                        break;
                    case ELevel.Level_08_SearingCrags:
                        ReplaceNotes(EItems.KEY_OF_STRENGTH); //Works
                        ReplacePhobekin(EItems.PYROPHOBIC_WORKER);
                        ReplaceFlowerBed();
                        break;
                    case ELevel.Level_09_A_GlacialPeak:
                        break;
                    case ELevel.Level_09_B_ElementalSkylands:
                        ReplaceNotes(EItems.KEY_OF_SYMBIOSIS);
                        break;
                    case ELevel.Level_10_A_TowerOfTime:
                        break;
                    case ELevel.Level_11_A_CloudRuins:
                        ReplacePhobekin(EItems.ACROPHOBIC_WORKER);
                        break;
                    case ELevel.Level_12_UnderWorld:
                        ReplaceNotes(EItems.KEY_OF_CHAOS); //Works
                        break;
                    case ELevel.Level_14_CorruptedFuture:
                        ReplaceNotes(EItems.KEY_OF_COURAGE);
                        break;
                }


            }
        }


        //Hooks both Init and ReInit to the same function to check and modify sprites if required
        //At this time unsure if REINIT is required.
        internal static void LevelInitializer_InitDone(On.LevelInitializer.orig_InitDone orig, LevelInitializer self)
        {
            CourierLogger.Log("SPRITE REPLACER", $"Init level {LevelManager.Instance.GetCurrentLevelEnum()}");
            orig(self);
            LevelInitializer_Rando(self);
        }

        internal static void LevelInitializer_ReinitDone(On.LevelInitializer.orig_ReinitDone orig, LevelInitializer self)
        {
            CourierLogger.Log("SPRITE REPLACER", $"REINIT level {LevelManager.Instance.GetCurrentLevelEnum()}");
            orig(self);
            LevelInitializer_Rando(self);
        }

        internal static void Cutscene_OnCutsceneDone(On.Cutscene.orig_OnCutsceneDone orig, Cutscene self)
        {
            orig(self);

            if (Level.Instance != null)
                Level.Instance.UpdateObjectActivators();
        }
        //We also hook Necrophobic Cutscene, even though it inherits and calls Cutscene_OnCutsceneDone, it adds the phobekin afterwards meaning we need to refresh the sprite visibility
        internal static void NecrophobicWorkerCutscene_OnCutsceneDone(On.NecrophobicWorkerCutscene.orig_OnCutsceneDone orig, NecrophobicWorkerCutscene self)
        {
            orig(self);

            if (Level.Instance != null)
                Level.Instance.UpdateObjectActivators();
        }


        /// <summary>
        /// Replaces an In World Items sprite
        /// </summary>
        /// <param name="whichItem">which item we are replacing</param>
        public static void ReplaceInWorldItem(EItems whichItem)
        {
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(whichItem, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {whichItem}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;



            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                return;


            GameObject rootItem = null;
            foreach(AwardItemCutscene AIC in Component.FindObjectsOfType<AwardItemCutscene>())
            {
                if(AIC.item.Equals(whichItem))
                {
                    rootItem = AIC.transform.root.gameObject;
                    break;
                }
            }


            //Special case for SEASHELL
            if (whichItem.Equals(EItems.SEASHELL))
            {
                TakeSeaShellCutscene TSSC = Component.FindObjectOfType<TakeSeaShellCutscene>();
                rootItem = TSSC.transform.root.gameObject;
            }


            if (rootItem == null)
                throw new RandomizerException($"We failed to get the root gameobject for {whichItem}");


            if (InventoryManager.Instance.GetItemDefinition(Key) == null)
                throw new RandomizerException($"Could not get Item Definition for Item {Key}");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");



            Transform artItem = rootItem.transform.Find("Art");

            //Special case for SEASHELL
            if (whichItem.Equals(EItems.SEASHELL))
                artItem = rootItem.transform.Find("Art_8");


            if (artItem != null)
            {
                foreach(SpriteRenderer SR in artItem.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (whichItem.Equals(EItems.MAGIC_BOOTS) || whichItem.Equals(EItems.SEASHELL))
                    {
                        if (SR.gameObject.name.Contains("8"))
                        {
                            SR.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            SR.material.shader = Shader.Find("Sprites/8_Bits");
                        }
                        else if (SR.gameObject.name.Contains("16"))
                        {
                            SR.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            SR.material.shader = Shader.Find("Sprites/16_Bits");
                        }
                    }
                        
                    SR.sprite = inventorySprite;
                }
            }

        }

        /// <summary>
        /// Replaces the PowerThistle sprite section of the flower bed
        /// </summary>
        public static void ReplaceFlowerBed()
        {

            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(EItems.POWER_THISTLE, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {EItems.POWER_THISTLE}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;

            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                throw new RandomizerException($"Skipping replacing sprite for {EItems.POWER_THISTLE}, already aquired item ({Key})");


            SpriteRenderer origPowerThistle = null;
            //Find original FlowerBed
            GameObject origFlowerBed = GameObject.Find("/FlowerBed");
            if (origFlowerBed == null)
                throw new RandomizerException("Could not find original FlowerBed object");

            foreach (SpriteRenderer SR in origFlowerBed.GetComponentsInChildren<SpriteRenderer>())
            {
                CourierLogger.Log("SPRITE REPLACER", $"FLOWERBED SPRITE: {SR.sprite.name}");
                if (SR.sprite.name.Contains("SearingCrags_16_PowerThistle_"))
                {
                    origPowerThistle = SR;
                    break;
                }
            }

            if (origPowerThistle == null)
                throw new RandomizerException("Could not find original PowerThistle object");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");



            origPowerThistle.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            //Then we add our own Sprite Renderer and apply the correct shader depending on which dimension the original object should be in.
            GameObject newArt = new GameObject();
            SpriteRenderer newSprite = newArt.AddComponent<SpriteRenderer>();
            newSprite.sprite = inventorySprite;
            newSprite.sortingLayerID = origPowerThistle.sortingLayerID;
            newSprite.sortingOrder = origPowerThistle.sortingOrder;
            newSprite.sortingLayerName = origPowerThistle.sortingLayerName;
            newSprite.material.shader = Shader.Find("Sprites/16_Bits");

            //Setting up the sprite changes
            newArt.transform.SetParent(origPowerThistle.gameObject.transform);
            newArt.transform.localPosition = origPowerThistle.transform.localPosition + new Vector3(-0.75f, 1.0f, 0.0f);


            AddHasItem(newArt, EItems.POWER_THISTLE);



            CourierLogger.Log("SPRITE REPLACER", $"[TEST] Replaced original FlowerBed");
        }





        /// <summary>
        /// Replaces the Ruxxtin Ammy sprite section of Ruxtins Tomb
        /// </summary>
        public static void ReplaceRuxxtinRomb()
        {

            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(EItems.RUXXTIN_AMULET, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {EItems.RUXXTIN_AMULET}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;

            if (InventoryManager.Instance == null)
                throw new RandomizerException("Could not find the Inventory Manager");


            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                throw new RandomizerException($"Skipping replacing sprite for {EItems.DEMON_KING_CROWN}, already aquired item ({Key})");


            SpriteRenderer origRuxxtinAmmy = null;
            //Find original FlowerBed
            GameObject origRuxxtinTomb = GameObject.Find("/RuxxtinTomb");
            if (origRuxxtinTomb == null)
                throw new RandomizerException("Could not find original FlowerBed object");

            foreach (SpriteRenderer SR in origRuxxtinTomb.GetComponentsInChildren<SpriteRenderer>())
            {
                CourierLogger.Log("SPRITE REPLACER", $"TOMB SPRITE: {SR.sprite.name}");
                if (SR.sortingOrder == 4)//.sprite.name.Contains("SearingCrags_16_PowerThistle_"))
                {
                    origRuxxtinAmmy = SR;
                    break;
                }
            }

            if (origRuxxtinAmmy == null)
                throw new RandomizerException("Could not find original Ruxxtin Ammulet object");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");



            origRuxxtinAmmy.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            //Then we add our own Sprite Renderer and apply the correct shader depending on which dimension the original object should be in.
            GameObject newArt = new GameObject();
            SpriteRenderer newSprite = newArt.AddComponent<SpriteRenderer>();
            newSprite.sprite = inventorySprite;
            newSprite.sortingLayerID = origRuxxtinAmmy.sortingLayerID;
            newSprite.sortingOrder = origRuxxtinAmmy.sortingOrder;
            newSprite.sortingLayerName = origRuxxtinAmmy.sortingLayerName;
            newSprite.material.shader = Shader.Find("Sprites/16_Bits");

            //Setting up the sprite changes
            newArt.transform.SetParent(origRuxxtinAmmy.gameObject.transform);
            newArt.transform.localPosition = origRuxxtinAmmy.transform.localPosition + new Vector3(-0.0f, 3.0f, 0.0f);


            AddHasItem(newArt, EItems.RUXXTIN_AMULET);



            CourierLogger.Log("SPRITE REPLACER", $"[TEST] Replaced original Ruxxtin Ammy");
        }



        /// <summary>
        /// Replaces the Demon Crown sprite section of Ruxtins Tomb
        /// </summary>
        public static void ReplaceDemonCrown()
        {

            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(EItems.DEMON_KING_CROWN, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {EItems.DEMON_KING_CROWN}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;

            if (InventoryManager.Instance == null)
                throw new RandomizerException("Could not find the Inventory Manager");


            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                throw new RandomizerException($"Skipping replacing sprite for {EItems.DEMON_KING_CROWN}, already aquired item ({Key})");


            SpriteRenderer origCrownSprite = null;
            //Find original FlowerBed
            GameObject origDemonCrown = GameObject.Find("/BossFight/OutroCutscene/Crown");
            if (origDemonCrown == null)
                throw new RandomizerException("Could not find original FlowerBed object");

            origCrownSprite = origDemonCrown.GetComponent<SpriteRenderer>();

            if (origCrownSprite == null)
                throw new RandomizerException("Could not find original PowerThistle object");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");


            //origCrownSprite.sprite = inventorySprite;
            origCrownSprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            //Then we add our own Sprite Renderer and apply the correct shader depending on which dimension the original object should be in.
            GameObject newArt = new GameObject();
            //newArt.gameObject.transform.position = origDemonCrown.transform.position;
            SpriteRenderer newSprite = newArt.AddComponent<SpriteRenderer>();
            newSprite.sprite = inventorySprite;
            newSprite.sortingLayerID = origCrownSprite.sortingLayerID;
            newSprite.sortingOrder = origCrownSprite.sortingOrder;
            newSprite.sortingLayerName = origCrownSprite.sortingLayerName;
            

            GraphicDimensionSwap GDSS = newSprite.gameObject.AddComponent<GraphicDimensionSwap>();
            GDSS.spriteRenderer = newSprite;

            //Setting up the sprite changes
            newArt.transform.SetParent(origCrownSprite.gameObject.transform);
            newArt.transform.localPosition = new Vector3(-0.0f, 0.0f, 0.0f);


            AddHasItem(newArt, EItems.DEMON_KING_CROWN);

            Console.Write($"NEW ART SHOULD BE AT GLOBAL POSITION {newArt.transform.position} WITH LOCAL POSITION {newArt.transform.localPosition}");

            CourierLogger.Log("SPRITE REPLACER", $"[TEST] Replaced original Demon Crown");
        }





        /// <summary>
        /// Replaces the Demon Crown sprite section of Ruxtins Tomb
        /// </summary>
        public static void ReplaceFirefly()
        {

            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(EItems.FAIRY_BOTTLE, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {EItems.FAIRY_BOTTLE}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;

            if (InventoryManager.Instance == null)
                throw new RandomizerException("Could not find the Inventory Manager");


            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                throw new RandomizerException($"Skipping replacing sprite for {EItems.FAIRY_BOTTLE}, already aquired item ({Key})");


            SpriteRenderer origCrownSprite = null;
            //Find original FlowerBed
            GameObject origDemonCrown = GameObject.Find("/BossFight");
            if (origDemonCrown == null)
            {
                throw new RandomizerException("Could not find original Luciole object");
            }

            origCrownSprite = origDemonCrown.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();

            if (origCrownSprite == null)
                throw new RandomizerException("Could not find original Luciole Sprite object");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");


            //origCrownSprite.sprite = inventorySprite;
            origCrownSprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            //Then we add our own Sprite Renderer and apply the correct shader depending on which dimension the original object should be in.
            GameObject newArt = new GameObject();
            //newArt.gameObject.transform.position = origDemonCrown.transform.position;
            SpriteRenderer newSprite = newArt.AddComponent<SpriteRenderer>();
            newSprite.sprite = inventorySprite;
            newSprite.sortingLayerID = origCrownSprite.sortingLayerID;
            newSprite.sortingOrder = origCrownSprite.sortingOrder;
            newSprite.sortingLayerName = origCrownSprite.sortingLayerName;


            GraphicDimensionSwap GDSS = newSprite.gameObject.AddComponent<GraphicDimensionSwap>();
            GDSS.spriteRenderer = newSprite;

            //Setting up the sprite changes
            newArt.transform.SetParent(origCrownSprite.gameObject.transform);
            newArt.transform.localPosition = new Vector3(-0.0f, 0.0f, 0.0f);


            AddHasItem(newArt, EItems.FAIRY_BOTTLE);

            Console.Write($"NEW ART SHOULD BE AT GLOBAL POSITION {newArt.transform.position} WITH LOCAL POSITION {newArt.transform.localPosition}");

            CourierLogger.Log("SPRITE REPLACER", $"[TEST] Replaced original Demon Crown");
        }









        /// <summary>
        /// Replaces the sprite of the specified phobekin with its randomized items inventory icon
        /// </summary>
        /// <param name="whichPhobekin">Which phobekin to replace if a phebekin is found</param>
        public static void ReplacePhobekin(EItems whichPhobekin = EItems.NONE)
        {
            CourierLogger.Log("SPRITE REPLACER", "ENTERING REPLACE PHOBEKIN");
            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(whichPhobekin, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {whichPhobekin}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;
            if (InventoryManager.Instance == null)
                throw new RandomizerException("Could not find the Inventory Manager");


            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                return;


            CourierLogger.Log("SPRITE REPLACER", "SHOULD HAVE SOME OUTPUT");

            SpriteRenderer phobekinSpriteRenderer = null;

            switch (whichPhobekin)
            {
                case EItems.PYROPHOBIC_WORKER:
                case EItems.ACROPHOBIC_WORKER:
                case EItems.CLAUSTROPHOBIC_WORKER:
                    PhobekinCollectCutscene PCC = Component.FindObjectOfType<PhobekinCollectCutscene>();
                    //Find the sprite renderer that has the layer of enemy
                    SpriteRenderer[] allRender = PCC.transform.root.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer SR in allRender)
                    {
                        if (SR.sortingLayerName.Equals("Enemies"))
                        {
                            phobekinSpriteRenderer = SR;
                            break;
                        }
                    }


                    break;
                case EItems.NECROPHOBIC_WORKER:
                    NecrophobicWorkerCutscene NWC = Component.FindObjectOfType<NecrophobicWorkerCutscene>();
                    phobekinSpriteRenderer = NWC.GetComponentInChildren<SpriteRenderer>();
                    break;


            }

            if (phobekinSpriteRenderer == null)
                throw new RandomizerException($"We could not find a matching Sprite Renderer for {whichPhobekin}");



            if (InventoryManager.Instance.GetItemDefinition(Key) == null)
                throw new RandomizerException($"Could not get Item Definition for Item {Key}");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");



            phobekinSpriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            //Then we add our own Sprite Renderer and apply the correct shader depending on which dimension the original object should be in.
            GameObject newArt = new GameObject();
            SpriteRenderer newSprite = newArt.AddComponent<SpriteRenderer>();
            newSprite.sprite = inventorySprite;
            newSprite.sortingLayerID = phobekinSpriteRenderer.sortingLayerID;
            newSprite.sortingOrder = phobekinSpriteRenderer.sortingOrder;
            newSprite.sortingLayerName = phobekinSpriteRenderer.sortingLayerName;

                GraphicDimensionSwap GDSS = newSprite.gameObject.AddComponent<GraphicDimensionSwap>();
                GDSS.spriteRenderer = newSprite;

                //Setting up the sprite changes
                newArt.transform.SetParent(phobekinSpriteRenderer.gameObject.transform);
                newArt.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);


                AddHasItem(newArt, whichPhobekin);



            CourierLogger.Log("SPRITE REPLACER", $"We successfully? replaced {whichPhobekin} with {Key}");

        }


        /// <summary>
        /// Replaces the given note with its random sprite using its inventory icon from ItemDefinitions
        /// This avoids the need for custom assets to have loaded references of each actual ingame sprite
        /// </summary>
        /// <param name="whichNote">The note we are replacing, this determines which "findnote" case is used</param>
        public static void ReplaceNotes(EItems whichNote = EItems.NONE)
        {

            ////What Item should be placed here?
            LocationRO randoItemCheck;
            if (!RandomizerStateManager.Instance.IsLocationRandomized(whichNote, out randoItemCheck))
                throw new RandomizerException($"Could not find a mapping for {whichNote}");


            EItems Key = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck].Item;
            if (InventoryManager.Instance == null)
                throw new RandomizerException("Could not find the Inventory Manager");


            if (InventoryManager.Instance.GetItemQuantity(Key) > 0)
                return;

            AwardNoteCutscene ANC = null;

            switch (whichNote)
            {
                //For some reason, HOPE has 2 game objects, one is inactive and never used... So we find and delete it BEFORE looking for the correct one.
                case EItems.KEY_OF_HOPE:
                    GameObject[] AllRoot = SceneManager.GetActiveScene().GetRootGameObjects();
                    foreach (GameObject HECK in AllRoot)
                    {
                        if ("KeyOvHope".Equals(HECK.name) && !HECK.activeInHierarchy)
                        {
                            GameObject.Destroy(HECK);
                            break;
                        }
                    }
                    ANC = Component.FindObjectOfType<AwardNoteCutscene>();
                    break;

                //A script deactivates the note and since we have no reference to it we iterate game objects
                case EItems.KEY_OF_LOVE:
                    //Find the Root Object that holds the inactive note
                    GameObject TO = GameObject.Find("/FX_SunMoonFire");
                    if (TO != null)
                    {
                        //Loop through all its transforms which includes inactive children
                        foreach (Transform dob in TO.transform)
                        {
                            //If we find the KeyOvLove child, we grab its AwardNoteCutscene
                            if ("KeyOvLove".Equals(dob.name))
                            {
                                ANC = dob.gameObject.GetComponent<AwardNoteCutscene>();
                                break;
                            }
                        }
                    }
                    break;
                //If we are the courage note we need a special cutscene
                case EItems.KEY_OF_COURAGE:
                    ANC = Component.FindObjectOfType<AwardKeyOfCourageCutscene>();
                    break;
                //If it is the Symbiosis note we need to find the outrocutscene for the boss
                case EItems.KEY_OF_SYMBIOSIS:
                    ElementalSkylandsBossOutroCutscene ESBOC = Component.FindObjectOfType<ElementalSkylandsBossOutroCutscene>();
                    ANC = ESBOC.awardNoteCutscene;
                    break;
                //These two work totally fine with just grabbing the AwardNoteCutscene
                case EItems.KEY_OF_STRENGTH:
                case EItems.KEY_OF_CHAOS:
                    ANC = Component.FindObjectOfType<AwardNoteCutscene>();
                    break;
                default:
                    throw new RandomizerException($"We could not find the note: {whichNote}");
            }




            //Check nothing failed before doing the replacement.
            if (ANC == null)
                throw new RandomizerException("We failed to find the note object");

            CourierLogger.Log("SPRITE REPLACER", $"WE FOUND THE NOTE {whichNote}");

            //At this point we have the correct Award Note Cutscene or we threw an exception.


            GameObject noteRoot = ANC.gameObject;

            if (InventoryManager.Instance.GetItemDefinition(Key) == null)
                throw new RandomizerException($"Could not get Item Definition for Item {Key}");

            ItemDefinition itemDef = InventoryManager.Instance.GetItemDefinition(Key);

            CourierLogger.Log("SPRITE REPLACER", $"HOLY SHIT NO ERROR {Key} - {noteRoot.name} - {itemDef.itemId}");

            Texture2D inventoryTex = itemDef.itemIcon;
            Sprite inventorySprite = Sprite.Create(inventoryTex, new Rect(0.0f, 0.0f, inventoryTex.width, inventoryTex.height), new Vector2(0.5f, 0.5f), 20.0f);
            if (inventorySprite == null)
                throw new RandomizerException($"We failed to create the sprite for {Key}");

            CourierLogger.Log("SPRITE REPLACER", "WE HAVE NOT ERRORED 1");
            SpriteRenderer SR = noteRoot.GetComponentInChildren<SpriteRenderer>();
            if (SR != null)
            {
                SR.sprite = inventorySprite;

                //Notes are always collectable even if normally invisible in 8bit, this makes them visible in all dimensions
                GraphicDimensionSwap GDS = SR.gameObject.AddComponent<GraphicDimensionSwap>();
                GDS.spriteRenderer = SR;

                //WE WANT BOTH DIM PARTICLES!
                ParticleSystemRenderer[] PSA = noteRoot.GetComponentsInChildren<ParticleSystemRenderer>();
                CourierLogger.Log("SPRITE REPLACER", "WE HAVE NOT ERRORED 3");
                foreach (ParticleSystemRenderer PS in PSA)
                {
                    GraphicDimensionSwap GDSS = PS.gameObject.AddComponent<GraphicDimensionSwap>();
                    GDSS.spriteRenderer = PS;
                }
 



                CourierLogger.Log("SPRITE REPLACER", $"We successfully? replaced {whichNote} with {Key}");
            }
            else
            {
                CourierLogger.Log("SPRITE REPLACER", "WE COULDNT FIND SR");
            }
        }

        /// <summary>
        /// Adds a has item check using an ObjectActivator that will hide or show the supplied game object based on the parameters set
        /// </summary>
        /// <param name="GO">The game object to monitor\add the check to</param>
        /// <param name="theItem">The Item</param>
        /// <param name="theOperator">The Operator</param>
        /// <param name="theQuantity">The Quantity</param>
        private static void AddHasItem(GameObject GO, EItems theItem, EConditionOperator theOperator = EConditionOperator.LESS_OR_EQUAL, int theQuantity = 0)
        {
            //Adds a HasItem for the current rando Item
            HasItem NHA = GO.AddComponent<HasItem>();
            NHA.item = theItem;
            NHA.conditionOperator = EConditionOperator.LESS_OR_EQUAL;
            NHA.quantityToHave = 0;

            //Sets up an Object Activator to hide the art once collected
            ObjectActivator OA = GO.AddComponent<ObjectActivator>();
            ConditionGroup CG = new ConditionGroup();
            ConditionList CL = new ConditionList();
            CL.conditionGroups = new List<ConditionGroup>();
            //Sets the owner of the HasItem to the ObjectActivator
            NHA.Owner = OA;
            //Adds the condition to the condition group, and the Condition List to the Object Activators condition list.
            CG.conditions.Add(NHA);
            CL.conditionGroups.Add(CG);
            OA.activeConditionList = CL;
            //Inits the ConditionGroup
            CG.Init();

            //Refreshes the initial ObjectActivator State
            if (Level.Instance != null)
                Level.Instance.UpdateObjectActivators();
        }
    }
}

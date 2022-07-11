using MessengerRando.RO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessengerRando.Utils
{
    /// <summary>
    /// A static class to handle replacement of Dialogs for items.
    /// Could be made better by adding the award text string to the LocationRO for each item 
    /// </summary>
    public static class DialogChanger
    {
        /// <summary>
        /// Gets each items rewards text that is found in Locale
        /// It is done this way as I don't know how to make a readonly constant dictionary?
        /// </summary>
        /// <returns>Dictionary of EItems and their Dialog string</returns>
        public static Dictionary<EItems, string> GetDialogIDtoItems()
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


        /// <summary>
        /// Gets the mapping of a dialog to its replacement
        /// </summary>
        /// <param name="dialogID"></param>
        /// <returns>the remapped dialogID. Returns original if not found</returns>
        public static string getDialogMapping(string dialogID)
        {
            Dictionary<string, string> mappings = RandomizerStateManager.Instance.CurrentLocationDialogtoRandomDialogMapping;
            if (mappings.ContainsKey(dialogID))
                return mappings[dialogID];
            else
                return dialogID;
        }

        /// <summary>
        /// The initial generation of the dictionary of dialog replacement based on the currently randomized item locations
        /// </summary>
        /// <returns>A Dictionary containing keys of locationdialogID and values of replacementdialogID</returns>
        /// 
        public static Dictionary<string, string> GenerateDialogMappingforItems()
        {
            Dictionary<string, string> dialogmap = new Dictionary<string, string>();
            Dictionary<EItems, string> ItemtoDialogIDMap = GetDialogIDtoItems();
            Dictionary<LocationRO, RandoItemRO> current = RandomizerStateManager.Instance.CurrentLocationToItemMapping;

            foreach (KeyValuePair<LocationRO, RandoItemRO> KVP in current)
            {
                Console.WriteLine($"Dialog mapping -- {KVP.Key.PrettyLocationName}");
                EItems LocationChecked = (EItems)Enum.Parse(typeof(EItems), KVP.Key.PrettyLocationName);
                RandoItemRO ItemActuallyFound = KVP.Value;

                if (ItemtoDialogIDMap.ContainsKey(LocationChecked) && ItemtoDialogIDMap.ContainsKey(ItemActuallyFound.Item))
                {
                    dialogmap.Add(ItemtoDialogIDMap[LocationChecked], ItemtoDialogIDMap[ItemActuallyFound.Item]);
                    Console.WriteLine($"We mapped item dialog {ItemtoDialogIDMap[ItemActuallyFound.Item]} to the location {ItemtoDialogIDMap[LocationChecked]}");
                }
            }
            return dialogmap;
        }

        /// <summary>
        /// Runs whenever the locale is loaded\changed. This should allow it to work in any language.
        /// Works by loading and replacing all dialogs and then using reflection to call the onlanguagechanged event on the localization manager to update all dialog to the correct text.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="language"></param>
        public static void LoadDialogs_Elanguage(On.DialogManager.orig_LoadDialogs_ELanguage orig, DialogManager self, ELanguage language)
        {

            //Loads all the original dialog
            orig(self, language);

            if (RandomizerStateManager.Instance.IsRandomizedFile && RandomizerStateManager.Instance.CurrentLocationDialogtoRandomDialogMapping != null)
            {
                //Sets the field info so we can use reflection to get and set the private field.
                FieldInfo dialogByLocIDField = typeof(DialogManager).GetField("dialogByLocID", BindingFlags.NonPublic | BindingFlags.Instance);

                //Gets all loaded dialogs and makes a copy
                Dictionary<string, List<DialogInfo>> Loc = dialogByLocIDField.GetValue(self) as Dictionary<string, List<DialogInfo>>;
                Dictionary<string, List<DialogInfo>> LocCopy = new Dictionary<string, List<DialogInfo>>(Loc);


                //Before we randomize get some fixed GOT ITEM text to replace text for Phoebekins
                List<DialogInfo> awardTextDialogList = Manager<DialogManager>.Instance.GetDialog("AWARD_GRIMPLOU");
                string awardText = awardTextDialogList[0].text;
                int replaceindexstart = awardText.IndexOf(">", 1);
                int replaceindexend = awardText.IndexOf("<", replaceindexstart);
                string toreplace = awardText.Substring(replaceindexstart + 1, replaceindexend - replaceindexstart - 1);

                //Phobekin text
                string phobeText = Manager<LocalizationManager>.Instance.GetText("UI_PHOBEKINS_TITLE").ToLower();
                phobeText = char.ToUpper(phobeText[0]) + phobeText.Substring(1); //Ugly way to uppercase the first letter.


                //Load the randomized mappings for an IF check so it doesn't run randomizer logic and replace itself with itself.
                Dictionary<string, string> dialogMap = RandomizerStateManager.Instance.CurrentLocationDialogtoRandomDialogMapping;

                //Loop through each dialog replacement - Will output the replacements to log for debugging
                foreach (KeyValuePair<string, List<DialogInfo>> KVP in Loc)
                {
                    string tobereplacedKey = KVP.Key;
                    string replacewithKey = DialogChanger.getDialogMapping(tobereplacedKey);


                    if (dialogMap.ContainsKey(tobereplacedKey))
                    {
                        //Replaces the entire dialog
                        LocCopy[tobereplacedKey] = Loc[replacewithKey];

                        //Sets them to be all center and no portrait (This really only applies to phobekins but was 
                        LocCopy[tobereplacedKey][0].autoClose = false;
                        LocCopy[tobereplacedKey][0].autoCloseDelay = 0;
                        LocCopy[tobereplacedKey][0].characterDefinition = null;
                        LocCopy[tobereplacedKey][0].forcedPortraitOrientation = 0;
                        LocCopy[tobereplacedKey][0].position = EDialogPosition.CENTER;
                        LocCopy[tobereplacedKey][0].skippable = true;

                        //This will replace the dialog for a phobekin to be its name in an award text
                        switch (replacewithKey)
                        {
                            case "FIND_ACRO":
                                string acro = Manager<LocalizationManager>.Instance.GetText("PHOBEKIN_ACRO_NAME");
                                acro = acro.Replace("<color=#00fcfc>", "");
                                acro = acro.Replace("</color>", "");
                                LocCopy[tobereplacedKey][0].text = awardText.Replace(toreplace, acro + " " + phobeText);
                                break;
                            case "FIND_PYRO":
                                string pyro = Manager<LocalizationManager>.Instance.GetText("PHOBEKIN_PYRO_NAME");
                                pyro = pyro.Replace("<color=#00fcfc>", "");
                                pyro = pyro.Replace("</color>", "");
                                LocCopy[tobereplacedKey][0].text = awardText.Replace(toreplace, pyro + " " + phobeText);
                                break;
                            case "FIND_CLAUSTRO":
                                string claustro = Manager<LocalizationManager>.Instance.GetText("PHOBEKIN_CLAUSTRO_NAME");
                                claustro = claustro.Replace("<color=#00fcfc>", "");
                                claustro = claustro.Replace("</color>", "");
                                LocCopy[tobereplacedKey][0].text = awardText.Replace(toreplace, claustro + " " + phobeText);
                                break;
                            case "NECRO_PHOBEKIN_DIALOG":
                                string necro = Manager<LocalizationManager>.Instance.GetText("PHOBEKIN_NECRO_NAME");
                                necro = necro.Replace("<color=#00fcfc>", "");
                                necro = necro.Replace("</color>", "");
                                LocCopy[tobereplacedKey][0].text = awardText.Replace(toreplace, necro + " " + phobeText);
                                break;
                        }

                        //This will remove all additional dialog that comes after the initial reward text
                        for (int i = LocCopy[tobereplacedKey].Count - 1; i > 0; i--)
                        {
                            LocCopy[tobereplacedKey].RemoveAt(i);
                        }
                    }
                }
                //Sets the replacements
                dialogByLocIDField.SetValue(self, LocCopy);

                //There is probably a better way to do this but I chose to use reflection to call all onLanguageChanged events to update the localization completely.
                if (Manager<LocalizationManager>.Instance != null)
                {
                    Type type = typeof(LocalizationManager);
                    FieldInfo field = type.GetField("onLanguageChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                    MulticastDelegate eventDelegate = field.GetValue(Manager<LocalizationManager>.Instance) as MulticastDelegate;


                    if (eventDelegate != null)
                    {
                        foreach (Delegate eventHandler in eventDelegate.GetInvocationList())
                        {
                            eventHandler.Method.Invoke(eventHandler.Target, null);
                        }
                    }
                }
            }
        }
    }
}

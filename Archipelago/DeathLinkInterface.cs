using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace MessengerRando.Archipelago
{
    public class DeathLinkInterface
    {
        public DeathLinkService DeathLinkService;
        public DeathLinkInterface Instance;
        public PlayerController Player;
        private List<DeathLink> DeathLinks = new List<DeathLink>();
        private bool _receivedDeath = false;
        private System.Random random = new System.Random();

        public DeathLinkInterface()
        {
            try
            {
                Console.WriteLine($"Initializing death link service... Should be set to {ArchipelagoData.DeathLink}");
                DeathLinkService = ArchipelagoClient.Session.CreateDeathLinkService();
                DeathLinkService.OnDeathLinkReceived += DeathLinkReceived;

                if (ArchipelagoData.DeathLink)
                    DeathLinkService.EnableDeathLink();
                else
                    DeathLinkService.DisableDeathLink();
            }
            catch { }
            
        }

        public void DeathLinkReceived(DeathLink deathLink)
        {
            _receivedDeath = true;
            DeathLinks.Add(deathLink);
            Console.WriteLine($"Received Death Link from: {deathLink.Source} due to {deathLink.Cause}");
        }

        public void KillPlayer()
        {
            try
            {
                if (DeathLinks.Count > 0 && _receivedDeath)
                {
                    var cause = DeathLinks[0].Cause;
                    if (cause.IsNullOrEmpty())
                    {
                        cause = DeathLinks[0].Source + DeathLinkCauses[random.Next(DeathLinkCauses.Count)];
                    }
                    DialogSequence receivedDeath = ScriptableObject.CreateInstance<DialogSequence>();
                    receivedDeath.dialogID = "DEATH_LINK";
                    receivedDeath.name = cause;
                    receivedDeath.choices = new List<DialogSequenceChoice>();
                    AwardItemPopupParams receivedDeathParams = new AwardItemPopupParams(receivedDeath, false);
                    Manager<UIManager>.Instance.ShowView<AwardItemPopup>(EScreenLayers.PROMPT, receivedDeathParams, true);
                    Player.Kill(EDeathType.GENERIC, null);
                    DeathLinks.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            _receivedDeath = false;
        }

        public void SendDeathLink(EDeathType type, Transform killedBy)
        {
            try
            {
                if (ArchipelagoData.DeathLink && !_receivedDeath)
                {
                    Console.WriteLine("Sharing death with your friends...");
                    var alias = ArchipelagoClient.Session.Players.GetPlayerAliasAndName(ArchipelagoClient.Session.ConnectionInfo.Slot);
                    var cause = DeathLinkCauses[random.Next(DeathLinkCauses.Count)];
                    DeathLinkService.SendDeathLink(new DeathLink(ArchipelagoClient.ServerData.SlotName, alias + cause));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        public static List<string> DeathLinkCauses = new List<string>()
        {
            " dropped their message.",
            " forgot to jump.",
            " just wanted to see Quarble.",
            " thought it was a simulation.",
            " blames that on input delay.",
        };
    }
}

using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WebSocketSharp;
using Object = UnityEngine.Object;

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
        private int deathsSent;
        private List<string> GenericDeathCauses;
        private List<string> ProjectileDeathCauses;
        private List<string> SpikeDeathCauses;
        private List<string> PitfallDeathCauses;
        private List<string> SquishDeathCauses;
        private List<string> FrequentDeathCauses;

        public DeathLinkInterface()
        {
            try
            {
                Console.WriteLine($"Initializing death link service... Should be set to {ArchipelagoData.DeathLink}");
                DeathLinkService = ArchipelagoClient.Session.CreateDeathLinkService();
                DeathLinkService.OnDeathLinkReceived += DeathLinkReceived;

                if (ArchipelagoData.DeathLink)
                {
                    DeathLinkService.EnableDeathLink();
                    GenerateFunnyCauses();
                }
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
                if (DeathLinks.Count > 0) _receivedDeath = true;
                if (!_receivedDeath) return;
                var cause = DeathLinks[0].Cause;
                if (cause.IsNullOrEmpty())
                {
                    cause = DeathLinks[0].Source + " sent you pain from afar.";
                }
                DialogSequence receivedDeath = ScriptableObject.CreateInstance<DialogSequence>();
                receivedDeath.dialogID = "DEATH_LINK";
                receivedDeath.name = cause;
                receivedDeath.choices = new List<DialogSequenceChoice>();
                AwardItemPopupParams receivedDeathParams = new AwardItemPopupParams(receivedDeath, false);
                Manager<UIManager>.Instance.ShowView<AwardItemPopup>(EScreenLayers.PROMPT, receivedDeathParams, true);
                Player.Kill(EDeathType.GENERIC, null);
                DeathLinks.RemoveAt(0);
                _receivedDeath = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendDeathLink(EDeathType deathType)
        {
            try
            {
                if (!ArchipelagoData.DeathLink || _receivedDeath) return;
                deathsSent++;
                Console.WriteLine("Sharing death with your friends...");
                var alias = ArchipelagoClient.Session.Players.GetPlayerAliasAndName(ArchipelagoClient.Session.ConnectionInfo.Slot);
                var cause = GetDeathLinkCause(deathType);
                DeathLinkService.SendDeathLink(new DeathLink(ArchipelagoClient.ServerData.SlotName, alias + cause));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        private void GenerateFunnyCauses()
        {
            FieldInfo stolenShardsCount =
                typeof(Quarble).GetField("timeShardList", BindingFlags.NonPublic | BindingFlags.Instance);
            GenericDeathCauses = new List<string> 
            {
                " dropped their message.",
                " forgot to jump.",
                " just wanted to see Quarble.",
                " thought it was a simulation.",
                " blames that on input delay.",
                $" has had their life saved {deathsSent} times.",
                $" has given Quarble {stolenShardsCount?.GetValue(Object.FindObjectOfType<Quarble>()) : 0} Time Shards.",
                " zigged when they should have zagged.",
                " let Quarble win another bet.",
                " wasn't able to get a selfie with Quarble.",
                " meant to do that.",
                " is playing with their feet.",
            };
            
            ProjectileDeathCauses = new List<string>
            {
                " thought that was a health potion.",
                " didn't dodge.",
                " walked into that one.",
                " forgot the 5 d's of dealing with projectiles.",
                " visited the Shopkeeper, but they don't carry awareness upgrades.",
                " forgot to tell the enemies not to shoot The Messenger.",
            };

            SpikeDeathCauses = new List<string>
            {
                " didn't avoid the spikes.",
                " didn't watch their step.",
                " wanted to see how long until the death quotes loop.",
                " just fell.",
            };

            SquishDeathCauses = new List<string>
            {
                " died in a painful way.",
                " had to confirm that one.",
                " wasn't forgiven by the moving blocks.",
                " got crushed.",
                " was pressed.",
            };

            FrequentDeathCauses = new List<string>
            {
                " deserves a discount at this point.",
                " won't give Quarble a break.",
                " is considering the package offer.",
                " is keeping Quarble busy.",
                " is overheating Quarble's ring.",
                " is letting Quarble do all the work.",
                " is keeping Quarble from his family.",
            };
        }

        private string GetDeathLinkCause(EDeathType deathType)
        {
            switch (deathType)
            {
                case EDeathType.INTRO:
                    return " is a new customer!";
                case EDeathType.GENERIC:
                    return GenericDeathCauses[random.Next()];
                case EDeathType.PROJECTILE:
                    return ProjectileDeathCauses[random.Next()];
                case EDeathType.SPIKES:
                    return SpikeDeathCauses[random.Next()];
                case EDeathType.PITFALL:
                    return PitfallDeathCauses[random.Next()];
                case EDeathType.SQUISH:
                    return SquishDeathCauses[random.Next()];
                case EDeathType.FREQUENT:
                    return FrequentDeathCauses[random.Next()];
                default:
                    throw new ArgumentOutOfRangeException(nameof(deathType));
            }
        }
    }
}

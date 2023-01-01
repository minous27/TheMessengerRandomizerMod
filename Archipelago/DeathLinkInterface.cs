using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using UnityEngine;

namespace MessengerRando.Archipelago
{
    public class DeathLinkInterface
    {
        public static DeathLinkService DeathLinkService;
        private static bool _deathLinkKilling;
        public DeathLinkInterface Instance;

        public static void Initialize()
        {
            try
            {
                Console.WriteLine("Initializing death link service...");
                DeathLinkService = ArchipelagoClient.Session.CreateDeathLinkService();
                DeathLinkService.OnDeathLinkReceived += DeathLinkReceived;
                if (ArchipelagoClient.ServerData.DeathLink)
                    DeathLinkService.EnableDeathLink();
                else
                    DeathLinkService.DisableDeathLink();
            }
            catch { }
            
        }

        public static void DeathLinkReceived(DeathLink deathLink)
        {
            _deathLinkKilling = true;
            Console.WriteLine($"Received Death Link from: {deathLink.Source} due to {deathLink.Cause}");
            Manager<PlayerManager>.Instance.Player.Kill(EDeathType.GENERIC, null);
        }

        public static void SendDeathLink(On.PlayerController.orig_Die orig, PlayerController self, EDeathType type, Transform killedBy)
        {
            if (!_deathLinkKilling)
            {
                if (ArchipelagoClient.ServerData.DeathLink)
                {
                    Console.WriteLine("Sharing the death...");
                    DeathLinkService.SendDeathLink(new DeathLink(ArchipelagoClient.ServerData.SlotName, killedBy.name));
                }
            }

            _deathLinkKilling = false;
            orig(self, type, killedBy);
        }
    }
}

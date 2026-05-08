using MessengerRando.RO;
using MessengerRando.Utils;
using Mod.Courier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MessengerRando.Scripts
{
    /// <summary>
    /// A MonoBehaviour added by SpriteReplacer with the LocationRO given
    /// Checks for the collection of the rando item pointed to in the LocationRO and sets the gameobject this script is attached to, to be inactive.
    /// </summary>
    public class HasRandoItem : MonoBehaviour
    {
        public float checkDelay = 1.0f;

        public LocationRO randoItemCheck;
        
        Coroutine CR;
        void OnEnable()
        {
            if (randoItemCheck != null)
            {
                CR = StartCoroutine(CheckForIDRoutine(randoItemCheck));
            }
        }

        void Start()
        {
            if (CR != null)
            {
                CR = StartCoroutine(CheckForIDRoutine(randoItemCheck));
            }
        }
        
        IEnumerator CheckForIDRoutine(LocationRO randoItemCheck)
        {
            RandoItemRO randoItem = RandomizerStateManager.Instance.CurrentLocationToItemMapping[randoItemCheck];
            bool found = false;
            while (found == false)
            {
                //This works for timeshards, but does not work on items
                if (randoItem.Item.Equals(EItems.TIME_SHARD)){
                    if (RandomizerStateManager.Instance.GetSeedForFileSlot(RandomizerStateManager.Instance.CurrentFileSlot).CollectedItems.Contains(randoItem))
                        found = true;
                }
                else
                {
                    if(InventoryManager.Instance.GetItemQuantity(randoItem.Item) > 0)
                        found = true;
                }
                yield return new WaitForSeconds(checkDelay);
            }
            this.gameObject.SetActive(false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{

    [Serializable]
    public class InventoryData
    {
        #region Consts
        public const int WEAPON_SLOT_ID = -1;
        public const int CHESTPLATE_SLOT_ID = -2;
        public const int BOOTS_SLOT_ID = -3;
        #endregion
        public int SlotCount;
        public InventorySlot equippedWeapon;
        public InventorySlot equippedChestplate;
        public InventorySlot equippedBoots;
        public List<InventorySlot> Slots = new();

        public bool HasSpace => Slots.Any(x => x.IsEmpty);
        public InventoryData(int slotCount)
        {
            SlotCount = slotCount;

            for (int i = 0; i < slotCount; i++)
            {
                Slots.Add(new InventorySlot(i));
            }
            equippedWeapon = new InventorySlot(WEAPON_SLOT_ID);
            equippedChestplate = new InventorySlot(CHESTPLATE_SLOT_ID);
            equippedBoots = new InventorySlot(BOOTS_SLOT_ID);
        }
    }
    [Serializable]
    public class InventorySlot
    {
        public int SlotIndex;
        public string ItemId;
        public int Quantity;

        public bool IsEmpty => string.IsNullOrEmpty(ItemId) || Quantity <= 0;

        public InventorySlot()
        {
        }
        public InventorySlot(int slotIndex)
        {
            SlotIndex = slotIndex;
        }
        public void Clear()
        {
            ItemId = null;
            Quantity = 0;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventoryData
    {
        public int SlotCount;
        public List<InventorySlot> Slots = new();

        public InventoryData(int slotCount)
        {
            SlotCount = slotCount;

            for (int i = 0; i < slotCount; i++)
            {
                Slots.Add(new InventorySlot(i));
            }
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
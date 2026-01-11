using Entities.Player;
using InventorySystem;
using InventorySystem.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] int slotCount = 20;
        public InventoryData Data { get; private set; }
        ItemDatabase database;
        Player player;

        public Action OnContentChanged;
        private void Awake()
        {
            database = ItemDatabase.Instance;
            Data = new InventoryData(slotCount);
        }
        private void Start()
        {
            player = GameManager.Player;
            InitializeEquipment();
        }

        #region Item Handle
        public bool AddItem(string itemId, int amount = 1) => AddItem(database.Get(itemId), amount);
        public bool AddItem(ItemObject item, int amount = 1)
        {
            if (item == null) return false;

            if (item.Stackable)
            {
                foreach (var slot in Data.Slots)
                {
                    if (slot.ItemId == item.Id && slot.Quantity < item.MaxStack)
                    {
                        int space = item.MaxStack - slot.Quantity;
                        int toAdd = Mathf.Min(space, amount);

                        slot.Quantity += toAdd;
                        amount -= toAdd;

                        if (amount <= 0)
                        {
                            OnContentChanged?.Invoke();
                            return true;
                        }
                    }
                }
            }

            foreach (var slot in Data.Slots)
            {
                if (slot.IsEmpty)
                {
                    slot.ItemId = item.Id;
                    slot.Quantity = Mathf.Min(amount, item.MaxStack);
                    OnContentChanged?.Invoke();
                    return true;
                }
            }

            //Inventory full
            return false;
        }

        public void RemoveItem(int slotIndex, int amount)
        {
            var slot = Data.Slots[slotIndex];
            slot.Quantity -= amount;

            if (slot.Quantity <= 0)
                slot.Clear();
            OnContentChanged?.Invoke();
        }
        public void MoveItem(int from, int to)
        {
            if (from == to) return;

            //Inventory > Inventory
            if (from >= 0 && to >= 0)
            {
                SwapInventorySlots(from, to);
                return;
            }

            //Inventory > Equipment
            if (from >= 0 && to < 0)
            {
                var fromItem = ItemDatabase.Instance.Get(Data.Slots[from].ItemId);
                if (fromItem is not EquipmentObject equipment)
                    return;
                if (InventoryUI.Instance.TryGetEquipmentTypeFromSlot(to, out EquipmentType type))
                {
                    if (equipment.Type != type)
                        return;
                    EquipItem(equipment, from);
                }
            }
            //Equipment > Inventory
            if (from < 0 && to >= 0)
            {
                if (InventoryUI.Instance.TryGetEquipmentTypeFromSlot(from, out EquipmentType type))
                {
                    switch (type)
                    {
                        case EquipmentType.Weapon:
                            UnequipItem(Data.equippedWeapon.ItemId);
                            break;
                        case EquipmentType.Chestplate:
                            UnequipItem(Data.equippedChestplate.ItemId);
                            break;
                        case EquipmentType.Boots:
                            UnequipItem(Data.equippedBoots.ItemId);
                            break;
                    }
                }

            }
        }

void SwapInventorySlots(int from, int to)
        {
            var slots = Data.Slots;
            var source = slots[from];
            var target = slots[to];

            if (source.IsEmpty) return;

            Debug.Log($"Swapping {from} to {to}");



            //Stack
            if (!target.IsEmpty && source.ItemId == target.ItemId)
            {
                var item = ItemDatabase.Instance.Get(source.ItemId);
                if (item.Stackable)
                {
                    int space = item.MaxStack - target.Quantity;
                    int moveAmount = Mathf.Min(space, source.Quantity);

                    target.Quantity += moveAmount;
                    source.Quantity -= moveAmount;

                    if (source.Quantity <= 0)
                        source.Clear();

                    return;
                }
            }

            //Swap
            (slots[from], slots[to]) = (slots[to], slots[from]);

            //Change Index
            slots[from].SlotIndex = from;
            slots[to].SlotIndex = to;
        }
        #endregion
        #region Equipment Handle
        void InitializeEquipment()
        {
            if (GameManager.Player.PlayerSettings.starterWeapon != null)
                EquipItem(GameManager.Player.PlayerSettings.starterWeapon);
            if (GameManager.Player.PlayerSettings.starterChestplate != null)
                EquipItem(GameManager.Player.PlayerSettings.starterChestplate);
            if (GameManager.Player.PlayerSettings.starterBoots!= null)
                EquipItem(GameManager.Player.PlayerSettings.starterBoots);
            InventoryUI.Instance.Refresh();
        }
        public void EquipItem(string equipmentId, int slotId = -1) => EquipItem(database.Get(equipmentId) as EquipmentObject, slotId);
        public void EquipItem(EquipmentObject equipment, int slotId = -1)
        {
            if (equipment == null) return;

            if (slotId != -1) //Item is in inventory
                RemoveItem(slotId, 1);

            switch (equipment)
            {
                case WeaponObject weapon:
                    if (!Data.equippedWeapon.IsEmpty)
                        UnequipItem(Data.equippedWeapon.ItemId);
                    Data.equippedWeapon.ItemId = weapon.Id;
                    Data.equippedWeapon.Quantity = 1;
                    break;
                case ChestplateObject chestplate:
                    if (!Data.equippedChestplate.IsEmpty)
                        UnequipItem(Data.equippedChestplate.ItemId);
                    Data.equippedChestplate.ItemId = chestplate.Id;
                    Data.equippedChestplate.Quantity = 1;
                    break;
                case BootsObject boots:
                    if (!Data.equippedBoots.IsEmpty)
                        UnequipItem(Data.equippedBoots.ItemId);
                    Data.equippedBoots.ItemId = boots.Id;
                    Data.equippedBoots.Quantity = 1;
                    break;
            }
            equipment.OnEquip(player);
        }
        public void UnequipItem(string equipmentId) => UnequipItem(database.Get(equipmentId) as EquipmentObject);
        public void UnequipItem(EquipmentObject equipment)
        {
            if (equipment == null) return;
            equipment.OnUnequip(player);
            switch (equipment.Type)
            {
                case EquipmentType.Weapon:
                    Data.equippedWeapon.Clear();
                    break;
                case EquipmentType.Chestplate:
                    Data.equippedChestplate.Clear();
                    break;
                case EquipmentType.Boots:
                    Data.equippedBoots.Clear();
                    break;
                default:
                    Debug.LogWarning($"Trying to unequip invalid equipment type");
                    break;
            }
            AddItem(equipment);
        }
        #endregion
    }
}
using Entities.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] int slotCount = 20;
        [SerializeField] WeaponObject equippedWeapon;
        [SerializeField] ChestplateObject equippedChestplate;
        [SerializeField] BootsObject equippedBoots;
        public InventoryData Data { get; private set; }
        ItemDatabase database;
        Player player;
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
        public bool AddItem(string itemId, int amount)
        {
            ItemObject item = database.Get(itemId);
            if (item == null) return false;

            if (item.Stackable)
            {
                foreach (var slot in Data.Slots)
                {
                    if (slot.ItemId == itemId && slot.Quantity < item.MaxStack)
                    {
                        int space = item.MaxStack - slot.Quantity;
                        int toAdd = Mathf.Min(space, amount);

                        slot.Quantity += toAdd;
                        amount -= toAdd;

                        if (amount <= 0)
                            return true;
                    }
                }
            }

            foreach (var slot in Data.Slots)
            {
                if (slot.IsEmpty)
                {
                    slot.ItemId = itemId;
                    slot.Quantity = Mathf.Min(amount, item.MaxStack);
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
        }
        #endregion
        #region Equipment Handle
        void InitializeEquipment()
        {
            if (equippedWeapon != null)
                EquipItem(equippedWeapon);
            if (equippedChestplate != null)
                EquipItem(equippedChestplate);
            if (equippedBoots != null)
                EquipItem(equippedBoots);
        }
        public void EquipItem(string equipmentId) => EquipItem(database.Get(equipmentId) as EquipmentObject);
        public void EquipItem(EquipmentObject equipment)
        {
            if (equipment == null) return;
            switch (equipment)
            {
                case WeaponObject weapon:
                    if (equippedWeapon != null)
                        equippedWeapon.OnUnequip(player);
                    equippedWeapon = weapon;
                    break;
                case ChestplateObject chestplate:
                    if (equippedChestplate != null)
                        equippedChestplate.OnUnequip(player);
                    equippedChestplate = chestplate;
                    break;
                case BootsObject boots:
                    if (equippedBoots != null)
                        equippedBoots.OnUnequip(player);
                    equippedBoots = boots;
                    break;
            }
            equipment.OnEquip(player);
        }
        public void UnequipItem(string equipmentId) => UnequipItem(database.Get(equipmentId) as EquipmentObject);
        public void UnequipItem(EquipmentObject equipment)
        {
            if (equipment == null) return;
            equipment.OnUnequip(player);
        }
        #endregion
    }
}
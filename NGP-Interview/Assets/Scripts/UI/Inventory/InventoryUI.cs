using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace InventorySystem.UI
{
    public class InventoryUI : Singleton<InventoryUI>
    {
        [SerializeField] Inventory inventory;
        [SerializeField] InventorySlotUI slotPrefab;
        [SerializeField] Transform slotsParent;
        [SerializeField] InventorySlotUI weaponSlot;
        [SerializeField] InventorySlotUI chestplateSlot;
        [SerializeField] InventorySlotUI bootsSlot;
        [SerializeField] GameObject dragIcon;

        private List<InventorySlotUI> slotUIs = new();

        public Inventory Inventory => inventory;
        public GameObject DragIcon => dragIcon;
        private void Start()
        {
            BuildUI();
            Refresh();
            inventory.OnContentChanged += Refresh;
        }

        private void BuildUI()
        {
            weaponSlot.Initialize(InventoryData.WEAPON_SLOT_ID, this);
            chestplateSlot.Initialize(InventoryData.CHESTPLATE_SLOT_ID, this);
            bootsSlot.Initialize(InventoryData.BOOTS_SLOT_ID, this);
            for (int i = 0; i < inventory.Data.SlotCount; i++)
            {
                var slotUI = Instantiate(slotPrefab, slotsParent);
                slotUI.Initialize(i, this);
                slotUIs.Add(slotUI);
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < slotUIs.Count; i++)
            {
                slotUIs[i].Refresh(inventory.Data.Slots[i]);
            }
            weaponSlot.Refresh(inventory.Data.equippedWeapon);
            chestplateSlot.Refresh(inventory.Data.equippedChestplate);
            bootsSlot.Refresh(inventory.Data.equippedBoots);
        }

        public void RequestMove(int from, int to)
        {
            inventory.MoveItem(from, to);
            Refresh();
        }

        public bool TryGetEquipmentTypeFromSlot(int slotId, out EquipmentType type)
        {
            type = default;

            return slotId switch
            {
                InventoryData.WEAPON_SLOT_ID => (type = EquipmentType.Weapon) == type,
                InventoryData.CHESTPLATE_SLOT_ID => (type = EquipmentType.Chestplate) == type,
                InventoryData.BOOTS_SLOT_ID => (type = EquipmentType.Boots) == type,
                _ => false
            };
        }

    }
}
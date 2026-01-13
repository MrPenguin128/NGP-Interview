using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] SlotType type;
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI quantityText;

        public int SlotIndex { get; private set; }

        private InventoryUI inventoryUI;
        private CanvasGroup canvasGroup;

        public void Initialize(int index, InventoryUI ui)
        {
            SlotIndex = index;
            inventoryUI = ui;

            canvasGroup = GetComponent<CanvasGroup>();
        }

        #region Refresh

        public void Refresh(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                icon.enabled = false;
                quantityText.text = "";
                return;
            }

            var item = ItemDatabase.Instance.Get(slot.ItemId);

            icon.enabled = true;
            icon.sprite = item.Icon;
            quantityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : "";
        }

        #endregion

        #region Drag

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (SlotIndex < 0)
            {
                inventoryUI.TryGetEquipmentTypeFromSlot(SlotIndex, out EquipmentType type);
                switch (type)
                {
                    case EquipmentType.Weapon:
                        if (inventoryUI.Inventory.Data.equippedWeapon.IsEmpty)
                            return;
                        break;
                    case EquipmentType.Chestplate:
                        if (inventoryUI.Inventory.Data.equippedChestplate.IsEmpty)
                            return;
                        break;
                    case EquipmentType.Boots:
                        if (inventoryUI.Inventory.Data.equippedBoots.IsEmpty)
                            return;
                        break;
                    default:
                        Debug.LogError($"Trying to drag an undefined type of equipment slot");
                        break;
                }
            }
            else if (inventoryUI.Inventory.Data.Slots[SlotIndex].IsEmpty)
                return;

            inventoryUI.DragIcon.transform.GetChild(0).GetComponent<Image>().sprite = icon.sprite;
            inventoryUI.DragIcon.SetActive(true);
            canvasGroup.alpha = 0.4f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (inventoryUI.DragIcon == null) return;
            inventoryUI.DragIcon.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.DragIcon.SetActive(false);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        #endregion

        #region Drop

        public void OnDrop(PointerEventData eventData)
        {
            var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
            if (draggedSlot == null || draggedSlot == this)
                return;
            inventoryUI.RequestMove(draggedSlot.SlotIndex, SlotIndex);
        }

        #endregion

        #region Tooltip
        public void OnPointerEnter(PointerEventData eventData)
        {
            InventorySlot slot;
            if (SlotIndex < 0)
            {
                if (inventoryUI.TryGetEquipmentTypeFromSlot(SlotIndex, out EquipmentType type))
                {
                    switch (type)
                    {
                        case EquipmentType.Weapon:
                            slot = inventoryUI.Inventory.Data.equippedWeapon;
                            break;
                        case EquipmentType.Chestplate:
                            slot = inventoryUI.Inventory.Data.equippedChestplate;
                            break;
                        case EquipmentType.Boots:
                            slot = inventoryUI.Inventory.Data.equippedBoots;
                            break;
                        default:
                            return;
                    }
                }
                else
                    return;
            }
            else
                slot = inventoryUI.Inventory.Data.Slots[SlotIndex];
            if (slot.IsEmpty)
                return;

            ItemTooltipUI.Instance.Show(ItemDatabase.Instance.Get(slot.ItemId));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ItemTooltipUI.Instance.Hide();
        }
        #endregion
        enum SlotType
        {
            EquipmentSlot,
            InventorySlot,
        }
    }
}
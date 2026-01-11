using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ChestplateObject", menuName = "Scriptable Objects/Inventory/Items/Chestplate")]
    public class ChestplateObject : EquipmentObject
    {
        private void Awake()
        {
            type = EquipmentType.Chestplate;
        }
    }
}
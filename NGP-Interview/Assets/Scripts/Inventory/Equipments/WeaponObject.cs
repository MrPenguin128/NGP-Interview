using Entities;
using Entities.Player;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "WeaponObject", menuName = "Scriptable Objects/Inventory/Items/Weapon")]
    public class WeaponObject : EquipmentObject
    {
        [SerializeField] WeaponTypes weaponType;
        [SerializeField] ComboDataObject comboData;
        public WeaponTypes WeaponType => weaponType;
        private void Awake()
        {
            type = EquipmentType.Weapon;
        }
        public override void OnEquip(BaseEntity entity)
        {
            base.OnEquip(entity);
            (entity as Player).SetComboData(comboData);
        }
        public override void OnUnequip(BaseEntity entity)
        {
            base.OnUnequip(entity);
            (entity as Player).SetComboData(null);
        }

        public enum WeaponTypes
        {
            Sword,
            Axe
        }
    }
}
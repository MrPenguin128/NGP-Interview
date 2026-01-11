using Entities;
using Entities.StatsSystem;
using UnityEngine;

namespace InventorySystem
{
    public class EquipmentObject : ItemObject
    {
        public StatModifier[] modifiers;
        private void Awake()
        {
            Stackable = false;
            MaxStack = 1;
        }
        public virtual void OnEquip(BaseEntity entity)
        {
            foreach (StatModifier modifier in modifiers)
            {
                entity.AddStatModifier(modifier);
            }
        }
        public virtual void OnUnequip(BaseEntity entity)
        {
            foreach (StatModifier modifier in modifiers)
            {
                entity.RemoveStatModifier(modifier);
            }

        }
    }
}
using InventorySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "Scriptable Objects/Player Settings")]
public class PlayerSettingsObject : ScriptableObject
{
    [Header("Base Stats Values")]
    public float BaseDamage;
    public float BaseMoveSpeed;
    public float BaseAttackSpeed;
    public float BaseAttackRange;
    public float BaseArmor;
    public float BaseCritChance;
    public float BaseCritDamage;
    [Header("Initial Equipment")]
    public WeaponObject starterWeapon;
    public ChestplateObject starterChestplate;
    public BootsObject starterBoots;
}

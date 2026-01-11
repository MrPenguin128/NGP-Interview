using Entities.Enemies;
using EntityStats;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Enemy Object", menuName = "Scriptable Objects/Enemies/Base Enemy")]
public class EnemyDataObject : ScriptableObject
{
    [Header("Base Stats Values")]
    [SerializeField] protected float baseMaxHealth;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseMoveSpeed;
    [SerializeField] protected float baseAttackSpeed;
    [SerializeField] protected float baseAttackRange;
    [SerializeField] protected float baseArmor;
    [SerializeField] protected float baseCritChance;
    [SerializeField] protected float baseCritDamage;
    [Header("Settings")]
    [SerializeField] Enemy enemyPrefab;

    public Enemy EnemyPrefab => enemyPrefab;
    public Dictionary<StatType, float> GetBaseStats()
    {
        return new Dictionary<StatType, float>
            {
                { StatType.MaxHealth, baseMaxHealth },
                { StatType.Damage, baseDamage },
                { StatType.MoveSpeed, baseMoveSpeed },
                { StatType.AttackSpeed, baseAttackSpeed },
                { StatType.AttackRange, baseAttackRange },
                { StatType.Armor, baseArmor },
                { StatType.CritChance, baseCritChance },
                { StatType.CritDamage, baseCritDamage },
            };
    }

}

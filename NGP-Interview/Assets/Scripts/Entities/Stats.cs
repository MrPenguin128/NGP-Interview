using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStats
{
    //Represents the container of all stats of a entity
    //Can have exclusive stats added, edited, and have modifiers added too
    [Serializable]
    public class Stats
    {
        [SerializeField] Dictionary<StatType, StatValue> stats = new();

        public void Initialize(Dictionary<StatType, float> baseStats)
        {
            stats.Clear();
            foreach (var stat in baseStats)
            {
                stats[stat.Key] = new StatValue { BaseValue = stat.Value };
            }
        }

        public float Get(StatType type)
        {
            if (!stats.TryGetValue(type, out var stat))
                return 0f;

            return stat.Value;
        }

        public void AddModifier(StatModifier modifier, bool createIfInexistent = true)
        {
            if (!stats.ContainsKey(modifier.Stat) && createIfInexistent)
                stats[modifier.Stat] = new StatValue();

            stats[modifier.Stat].AddModifier(modifier);
        }

        public void Tick(float deltaTime)
        {
            foreach (var stat in stats.Values)
            {
                stat.Tick(deltaTime);
            }
        }
    }

    //Represents a single stat
    //It has a base value and a list of modifiers
    //It is responsible to remove its own expired modifiers by the Tick method
    [Serializable]
    public class StatValue
    {
        public float BaseValue;
        List<StatModifier> modifiers = new List<StatModifier>();

        public float Value
        {
            get
            {
                float finalValue = BaseValue;
                float percentAdd = 0f;
                float percentMul = 1f;

                foreach (var mod in modifiers)
                {
                    switch (mod.Type)
                    {
                        case StatModifier.StatModifierType.Flat:
                            finalValue += mod.Value;
                            break;
                        case StatModifier.StatModifierType.PercentAdd:
                            percentAdd += mod.Value;
                            break;
                        case StatModifier.StatModifierType.PercentMul:
                            percentMul *= mod.Value;
                            break;
                        default:
                            break;
                    }
                }
                finalValue *= 1f + percentAdd;
                finalValue *= percentMul;

                return finalValue;
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        public void Tick(float deltaTime)
        {
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var mod = modifiers[i];
                if (!mod.IsTemporary)
                    continue;

                mod.RemainingTime -= deltaTime;
                if (mod.RemainingTime <= 0f)
                    modifiers.RemoveAt(i);
            }
        }
    }

    //Represents a Stat Modifier
    //It can affect any StatType, and can be permanent or not
    //The Stat is responsible to remove the modifier that expired
    //Each StatModifierType contribute in a different way to the stat value calculation.
    [Serializable]
    public class StatModifier
    {
        StatType stat;
        float value;
        StatModifierType type;
        float Duration; // <= 0 = permanent

        public StatType Stat => stat;
        public float Value => value;
        public StatModifierType Type => type;
        public float RemainingTime { get; set; }
        public bool IsTemporary => Duration > 0f;

        public StatModifier(StatType stat, float value, StatModifierType type, float duration = 0f)
        {
            this.stat = stat;
            this.value = value;
            this.type = type;
            Duration = duration;
            RemainingTime = duration;
        }
        public enum StatModifierType
        {
            Flat,       // +5
            PercentAdd, // +10%
            PercentMul  // *1.2
        }
    }

    public enum StatType
    {
        MaxHealth,
        Damage,
        MoveSpeed,
        AttackSpeed,
        Armor,
        CritChance,
    }

}
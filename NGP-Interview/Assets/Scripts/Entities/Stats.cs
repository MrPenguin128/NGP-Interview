using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Entities.StatsSystem
{
    //Represents the container of all stats of a entity
    //Can have exclusive stats added, edited, and have modifiers added too
    [Serializable]
    public class Stats
    {
        public List<StatPair> statsList = new List<StatPair>();

        public List<StatPair> AllStats => statsList;

        public void Initialize(Dictionary<StatType, float> baseStats)
        {
            statsList.Clear();
            foreach (var stat in baseStats)
            {
                statsList.Add(new StatPair(stat.Key, new StatValue { BaseValue = stat.Value }));
            }
        }

        public StatValue GetStat(StatType type)
        {
            for (int i = 0; i < statsList.Count; i++)
            {
                if (statsList[i].Type == type)
                    return statsList[i].Value;
            }
            return null;
        }
        public float GetValue(StatType type)
        {
            for (int i = 0; i < statsList.Count; i++)
            {
                if (statsList[i].Type == type)
                    return statsList[i].Value.Value;
            }
            return 0f;
        }

        public void AddModifier(StatModifier modifier, bool createIfInexistent = true)
        {
            if (!statsList.Any(x => x.Type == modifier.Stat) && createIfInexistent)
                statsList.Add(new StatPair(modifier.Stat, new StatValue()));

            statsList.Find(x => x.Type == modifier.Stat).Value.AddModifier(modifier);
        }
        public void RemoveModifier(StatModifier modifier)
        {
            var stat = statsList.Find(x => x.Value.Modifiers.Contains(modifier));
            if (stat != null)
                stat.Value.RemoveModifier(modifier);
        }

        public void Tick(float deltaTime)
        {
            foreach (var stat in statsList)
            {
                stat.Value.Tick(deltaTime);
            }
        }

        [Serializable]
        public class StatPair
        {
            public StatType Type;
            public StatValue Value;

            public StatPair(StatType key,StatValue newValue)
            {
                Type = key;
                Value = newValue;
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
        public List<StatModifier> Modifiers = new List<StatModifier>();

        public event Action<float> OnValueChanged;
        public float Value
        {
            get
            {
                float finalValue = BaseValue;
                float percentAdd = 0f;
                float percentMul = 1f;

                foreach (var mod in Modifiers)
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
            Modifiers.Add(modifier);
            OnValueChanged?.Invoke(Value);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            Modifiers.Remove(modifier);
            OnValueChanged?.Invoke(Value);
        }

        public void Tick(float deltaTime)
        {
            for (int i = Modifiers.Count - 1; i >= 0; i--)
            {
                var mod = Modifiers[i];
                if (!mod.IsTemporary)
                    continue;

                mod.RemainingTime -= deltaTime;
                if (mod.RemainingTime <= 0f)
                {
                    RemoveModifier(mod);
                }
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
        [SerializeField] StatType stat;
        [SerializeField] float value;
        [SerializeField] StatModifierType type;
        [SerializeField] float Duration; // <= 0 = permanent

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
        AttackRange,
        Armor,
        CritChance,
        CritDamage,
    }

}
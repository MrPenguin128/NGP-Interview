using Entities.StatsSystem;
using System.Collections.Generic;
using System.Globalization;

public static class ItemDescriptionParser
{
    private static readonly Dictionary<StatType, string> StatTags =
        new()
        {
            { StatType.MaxHealth, "{[HPM]}" },
            { StatType.Damage, "{[DMG]}" },
            { StatType.MoveSpeed, "{[MSP]}" },
            { StatType.AttackSpeed, "{[ASP]}" },
            { StatType.AttackRange, "{[RNG]}" },
            { StatType.Armor, "{[ARM]}" },
            { StatType.CritChance, "{[CRC]}" },
            { StatType.CritDamage, "{[CRD]}" },
        };

    public static string Parse(string rawDescription, IEnumerable<StatModifier> modifiers)
    {
        if (string.IsNullOrEmpty(rawDescription))
            return rawDescription;

        string result = rawDescription;

        foreach (var pair in StatTags)
        {
            float value = GetModifierValue(pair.Key, modifiers);

            if (value == 0f)
                continue;

            result = result.Replace(
                pair.Value,
                FormatValue(pair.Key, value));
        }

        return result;
    }

    private static float GetModifierValue(
        StatType stat,
        IEnumerable<StatModifier> modifiers)
    {
        float total = 0f;

        foreach (var mod in modifiers)
        {
            if (mod.Stat == stat)
                total += mod.Value;
        }

        return total;
    }

    private static string FormatValue(
        StatType stat,
        float value)
    {
        // Percentuais
        if (stat == StatType.CritChance ||
            stat == StatType.AttackSpeed)
        {
            return $"{value * 100f:0.#}%";
        }

        // Multiplicadores
        if (stat == StatType.CritDamage)
        {
            return $"{value:0.#}x";
        }

        // Valores numéricos normais
        return value.ToString("0.#");
    }
}

using System;
using System.Collections.Generic;

public enum StatusEffectType
{
    Poison,
    Stun,
    Buff,
    Debuff,
    Regeneration
}
public enum StatType
{
    Speed,
    Strength,
    Power,
    Aggression,
    Preservation
}

public class StatusEffect
{
    public StatusEffectType type;
    public int duration;
    public int power;
    public float multiplier;
    public bool hasBeenApplied = false;

    // only for buffs and debuffs
    // kinda same as normal - tuple is statType, power, modifier
    public List<Tuple<StatType, int, float>> statChanges = new();

    // used for Poison, Stun, Regen
    public StatusEffect(StatusEffectType type, int duration, int power = 0, float multiplier = 1f)
    {
        this.type = type;
        this.duration = duration;
        this.power = power;
        this.multiplier = multiplier;
    }

    // used for buffs and debuffs
    public StatusEffect(StatusEffectType type, int duration, List<Tuple<StatType, int, float>> statChanges, bool hasBeenApplied)
    {
        this.type = type;
        this.duration = duration;
        this.statChanges = statChanges;
        this.hasBeenApplied = hasBeenApplied;
    }
}

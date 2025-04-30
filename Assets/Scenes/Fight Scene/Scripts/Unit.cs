using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private UnitData data;
    public Bars bars;

    public List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

    private Collider2D col;

    private SpriteRenderer sr;
    private Color baseColor;
    private static readonly Color targetHighlightColor = Color.yellow;
    private static readonly Color currentTurnColor = Color.cyan;

    public Canvas unitCanvas;

    // from data
    public string unitName;

    public int maxHP;
    public int currentHP;
    public int currentEnergy;
    public int currentSpeed;
    public int currentStrength;
    public int currentPower;
    public int currentAggression;
    public int currentPreservation;

    public bool isFriendly;
    public bool isFrontlane;
    public bool isBacklane;

    public Sprite unitSprite;
    public Ability[] abilities;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) baseColor = sr.color;

        unitCanvas = GetComponentInChildren<Canvas>();
    }

    public void AdjustCanvas()
    {
        if (unitCanvas != null)
        {
            Vector3 currentPosCanvas = unitCanvas.transform.position;
            Vector3 currentPosUnit = transform.localPosition;

            float diff = Math.Abs(currentPosUnit.x - currentPosCanvas.x);
            unitCanvas.transform.position = new Vector3(currentPosUnit.x - diff, currentPosCanvas.y, currentPosCanvas.z);
        }
    }

    public void Init(UnitData unitData)
    {
        data = unitData;
        maxHP = data.tissue;
        currentHP = maxHP;
        currentEnergy = 0;
        currentSpeed = data.speed;
        currentStrength = data.strength;
        currentPower = data.power;
        currentAggression = data.aggression;
        currentPreservation = data.preservation;

        isFriendly = data.isFriendly;
        isFrontlane = data.isFrontlane;
        isBacklane = data.isBacklane;

        unitName = data.unitName;
        unitSprite = data.unitSprite;
        abilities = data.abilities;

        if (bars != null)
        {
            bars.SetMax(maxHP, 20);
            bars.setHealth(currentHP);
            bars.setEnergy(currentEnergy);
        }
    }

    public void TakeDamage(int damageTaken)
    {
        currentHP -= damageTaken;
        if (currentHP < 0) currentHP = 0;

        updateBars(currentHP, currentEnergy);
    }

    public void TakeEnergy(int energyTaken)
    {
        currentEnergy -= energyTaken;

        updateBars(currentHP, currentEnergy);
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        if (currentHP > maxHP)
            currentHP = maxHP;

        updateBars(currentHP, currentEnergy);
    }

    public bool IsAlive() { return currentHP > 0; }

    private void updateBars(int currentHP, int energy)
    {
        bars.setHealth(currentHP);
        bars.setEnergy(energy);
    }

    private void OnMouseDown()
    {
        if (!BattleUI.Instance.waitingForTarget) return;
        if (BattleUI.Instance.currentAbility == null) return;
        if (!IsAlive()) return;

        Unit caster = BattleUI.Instance.currentUnit;
        Ability ability = BattleUI.Instance.currentAbility;

        // Get valid targets
        List<Unit> validTargets = BattleManager.Instance.GetValidTargetsForAbility(caster, ability);

        // Check if this unit is a valid target
        if (validTargets.Contains(this))
        {
            // Add this unit to selected targets
            if (!BattleUI.selectedTargets.Contains(this))
            {
                BattleUI.selectedTargets.Add(this);
            }
        }
    }

    public void SetSelectable(bool selectable)
    {
        if (col != null)
            col.enabled = selectable;
    }

    public void HighlightAsTarget(bool highlight)
    {
        if (sr == null) return;

        sr.color = highlight ? targetHighlightColor : baseColor;
    }

    public void HighlightAsCurrentTurn(bool highlight)
    {
        if (sr == null) return;

        sr.color = highlight ? currentTurnColor : baseColor;
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        activeStatusEffects.Add(effect);
    }

    public void UpdateStatusEffects()
    {
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeStatusEffects[i];

            // Apply effect based on type
            switch (effect.type)
            {
                case StatusEffectType.Poison:
                    TakeDamage(Mathf.RoundToInt(effect.power * effect.multiplier)); // Apply poison damage
                    break;

                case StatusEffectType.Stun:
                    // Decrease energy by 10
                    TakeEnergy(Mathf.Max(0, currentEnergy - 10));
                    break;

                case StatusEffectType.Debuff:
                    if (!effect.hasBeenApplied)
                    {
                        ApplyStatChanges(effect.statChanges, false, true);
                        effect.duration++;
                        effect.hasBeenApplied = true;
                    }
                    break;

                case StatusEffectType.Buff:
                    if (!effect.hasBeenApplied)
                    {
                        ApplyStatChanges(effect.statChanges, true, true);
                        effect.duration++;
                        effect.hasBeenApplied = true;
                    }
                    break;

                case StatusEffectType.Regeneration:
                    Heal(Mathf.RoundToInt(effect.power * effect.multiplier)); // Apply regeneration healing
                    break;
            }

            // Decrease duration and remove if no durability

            effect.duration--;
            if (effect.duration <= 0)
            {
                if (effect.type == StatusEffectType.Buff) ApplyStatChanges(effect.statChanges, true, false);
                if (effect.type == StatusEffectType.Debuff) ApplyStatChanges(effect.statChanges, false , false);

                activeStatusEffects.RemoveAt(i);
            }
        }
    }

    // isBuff = true for buff, false for debuff, isAdd = true for adding modifier, false for removing
    public void ApplyStatChanges(List<Tuple<StatType, int, float>> statChanges, bool isBuff, bool isAdd)
    {
        foreach (var stat in statChanges)
        {
            var statType = stat.Item1;
            int amount = stat.Item2;
            float multi = stat.Item3;
            int finalVal;

            if (isAdd)
            {
                finalVal = Mathf.RoundToInt(stat.Item2 * stat.Item3) * (isBuff ? 1 : -1);
            }
            else
            {
                finalVal = Mathf.RoundToInt(stat.Item2 * stat.Item3) * (isBuff ? -1 : 1);
            }


            switch (statType)
            {
                case StatType.Speed:
                    currentSpeed += finalVal;
                    break;
                case StatType.Strength:
                    currentStrength += finalVal;
                    break;
                case StatType.Power:
                    currentPower += finalVal;
                    break;
                case StatType.Aggression:
                    currentAggression += finalVal;
                    break;
                case StatType.Preservation:
                    currentPreservation += finalVal;
                    break;
            }
        }
    }
}
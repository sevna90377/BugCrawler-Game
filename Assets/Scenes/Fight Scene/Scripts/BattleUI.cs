using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;
    public GameObject actionPanel;
    public Button[] abilityButtons;
    public static List<Unit> selectedTargets = new List<Unit>();
    public bool waitingForTarget = false;
    public Unit currentUnit;
    public bool actionChosen = false;
    public Ability currentAbility;
    public Unit chosenTarget;
    public Ability chosenAbility;

    private void Awake() => Instance = this;

    public void ShowActions(Unit unit)
    {
        currentUnit = unit;
        actionChosen = false;
        selectedTargets.Clear();

        actionPanel.SetActive(true);

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < unit.abilities.Length)
            {
                abilityButtons[i].gameObject.SetActive(true);
                var ability = unit.abilities[i];
                TMP_Text buttonText = abilityButtons[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = ability.abilityName;
                int index = i;
                abilityButtons[i].onClick.RemoveAllListeners();
                abilityButtons[i].onClick.AddListener(() => SelectAbility(index));
            }
            else
            {
                abilityButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideActions()
    {
        actionPanel.SetActive(false);
    }

    void SelectAbility(int index)
    {
        currentAbility = currentUnit.abilities[index];
        chosenAbility = currentAbility;
        StartCoroutine(TargetSelection());
    }

    IEnumerator TargetSelection()
    {
        HideActions();
        waitingForTarget = true;
        selectedTargets.Clear();

        // Get valid targets using centralized method
        List<Unit> validTargets = BattleManager.Instance.GetHighlightableTargets(currentUnit, currentAbility);
        bool isMultiTarget = BattleManager.Instance.IsMultiTargetAbility(currentAbility);

        // Highlight all valid targets
        foreach (var unit in BattleManager.Instance.allUnits)
        {
            bool isValid = validTargets.Contains(unit);
            unit.SetSelectable(isValid);
            unit.HighlightAsTarget(isValid);
        }

        // If multi-target ability, auto-select all valid targets when player selects any of them
        if (isMultiTarget)
        {
            // Wait for player to click on any valid target
            yield return new WaitUntil(() => selectedTargets.Count > 0);

            // If they selected a valid target, replace with the whole group
            if (selectedTargets.Count > 0 && validTargets.Contains(selectedTargets[0]))
            {
                selectedTargets.Clear();
                selectedTargets.AddRange(validTargets);
            }
        }
        else
        {
            // For single target, wait for player selection
            yield return new WaitUntil(() => selectedTargets.Count > 0);
        }

        // Reset highlighting
        foreach (var unit in BattleManager.Instance.allUnits)
        {
            unit.SetSelectable(false);
            unit.HighlightAsTarget(false);
        }

        // Apply ability to all selected targets
        foreach (var target in selectedTargets)
        {
            currentAbility.Activate(currentUnit, target);
        }

        waitingForTarget = false;
        actionChosen = true;
    }
}
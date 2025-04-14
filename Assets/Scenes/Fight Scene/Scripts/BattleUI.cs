using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public GameObject actionPanel;
    public Button[] abilityButtons;

    public static Unit selectedTarget = null;
    public bool waitingForTarget = false;

    public TextMeshProUGUI currentUnitNameText;

    private Unit currentUnit;
    public bool actionChosen = false;
    public Ability currentAbility;

    private void Awake() => Instance = this;

    public void ShowActions(Unit unit)
    {
        currentUnit = unit;
        actionChosen = false;
        actionPanel.SetActive(true);

        currentUnitNameText.text = $"{unit.data.unitName}'s Turn";

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            var ability = unit.data.abilities[i];
            TMP_Text buttonText = abilityButtons[i].GetComponentInChildren<TMP_Text>();
            buttonText.text = ability.abilityName;

            int index = i; // capture local index for closure
            abilityButtons[i].onClick.RemoveAllListeners();
            abilityButtons[i].onClick.AddListener(() => SelectAbility(index));
        }
    }

    public void HideActions()
    {
        actionPanel.SetActive(false);
    }

    void SelectAbility(int index)
    {
        Debug.Log("Selected ability index: " + index);
        currentAbility = currentUnit.data.abilities[index];
        StartCoroutine(TargetSelection(currentAbility));
    }

    IEnumerator TargetSelection(Ability ability)
    {
        HideActions();
        waitingForTarget = true;
        selectedTarget = null;

        Debug.Log("Choose a target...");

        yield return new WaitUntil(() => selectedTarget != null);

        ability.Activate(currentUnit, selectedTarget);
        selectedTarget = null;
        waitingForTarget = false;
        actionChosen = true;
    }
}

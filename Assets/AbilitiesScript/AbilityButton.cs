using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public BasicAbility basicAbility;
    private BattleSystem battleSystem;

    public void SetupButton(Unit unit, BasicAbility ability, BattleSystem bs)
    {
        this.basicAbility = ability;
        this.battleSystem = bs;

        Button button = GetComponent<Button>();
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button == null || buttonText == null)
        {
            Debug.LogWarning("Button or TextMeshProUGUI not found on AbilityButton.");
            return;
        }

        buttonText.text = ability.name;
        gameObject.SetActive(true);

        // Clear previous listeners to prevent stacking across turns
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            battleSystem.OnAbilityButtonClicked(basicAbility);
        });
    }
}

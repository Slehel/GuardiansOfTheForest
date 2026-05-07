using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public BasicAbility basicAbility;
    private BattleSystem battleSystem;

    public void SetupButton(Unit unit, BasicAbility ability, BattleSystem battleSystem)
    {
        this.basicAbility = ability;
        this.battleSystem = battleSystem;
        Button button = GetComponent<Button>();
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button != null && buttonText != null)
        {
            button.onClick.RemoveAllListeners();

            bool onCooldown = ability.IsOnCooldown();
            button.interactable = !onCooldown;
            buttonText.text = onCooldown
                ? ability.name + " (" + ability.currentCooldown + ")"
                : ability.name;

            button.onClick.AddListener(() =>
            {
                if (unit != null)
                    battleSystem.OnAbilityButtonClicked(basicAbility);
            });
        }
        else
        {
            Debug.LogWarning("Button or TextMeshProUGUI component not found on the button.");
        }
    }
}

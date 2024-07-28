using TMPro;
using UnityEditor.Playables;
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
            // Set the button text to the ability name
            buttonText.text = ability.name;

            // Add a listener to the button click event
            button.onClick.AddListener(() =>
            {
                if (unit != null)
                {
                    //basicAbility.useAbility(ability); ez a regi 
                    battleSystem.OnAbilityButtonClicked(basicAbility);
                }
            });
        }
        else
        {
            Debug.LogWarning("Button or TextMeshProUGUI component not found on the button.");
        }
    }
}

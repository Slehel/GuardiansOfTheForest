using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public int abilityIndex;
    CorruptionAbility corruptionAbility;
    OffensiveAbility offensiveAbility;
    HealingAbility healingAbility; 
    private CharacterAbilities characterAbilities;

    public void SetupButton(CharacterAbilities characterAbilities, Ability ability)
    {
        this.characterAbilities = characterAbilities;
        Button button = GetComponent<Button>();
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button != null && buttonText != null)
        {
            // Set the button text to the ability name
            buttonText.text = ability.properties.abilityName;

            // Add a listener to the button click event
            button.onClick.AddListener(() =>
            {
                if (characterAbilities != null)
                {
                    characterAbilities.UseAbility(abilityIndex);
                }
            });
        }
        else
        {
            Debug.LogWarning("Button or TextMeshProUGUI component not found on the button.");
        }
    }
}

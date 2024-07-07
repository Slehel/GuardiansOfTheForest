using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public BasicAbility basicAbility;

    public void SetupButton(CharacterAbilities characterAbilities, BasicAbility ability)
    {
        this.basicAbility = ability;
        Button button = GetComponent<Button>();
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button != null && buttonText != null)
        {
            // Set the button text to the ability name
            buttonText.text = ability.name;

            // Add a listener to the button click event
            button.onClick.AddListener(() =>
            {
                if (characterAbilities != null)
                {
                    basicAbility.useAbility(ability);
                }
            });
        }
        else
        {
            Debug.LogWarning("Button or TextMeshProUGUI component not found on the button.");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoader : MonoBehaviour
{
    public TextAsset jsonFile;
    public CharacterAbilities CharacterAbilities;

    void Start()
    {
        if (jsonFile != null)
        {

            AbilityList abilityList = JsonUtility.FromJson<AbilityList>(jsonFile.text);
            CharacterAbilities.abilities.Clear();
            foreach (string abilityName in CharacterAbilities.abilityNames)
            {
                foreach (BasicAbility ability in abilityList.abilities)
                {
                    if (ability.name == abilityName)
                    {
                        CharacterAbilities.abilities.Add(ability);
                        break;  // Exit the inner loop once a match is found
                    }
                }
            }

            // Log abilities of the character to verify
            string parentObjectName = CharacterAbilities.gameObject.name;
            Debug.Log("Character Abilities for " + parentObjectName + ":");
            foreach (var ability in CharacterAbilities.abilities)
            {
                Debug.Log("Ability Name: " + ability.name);
                // Debug.Log("Damage: " + ability.damage);
                // Debug.Log("Cooldown: " + ability.cooldown);
                // Debug.Log("Range: " + ability.range);
            }
        }
        else
        {
            Debug.LogError("JSON file not found!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoader : MonoBehaviour
{
    public TextAsset jsonFile;

    public void LoadAbilities(Unit unit)
    {
        if (jsonFile != null)
        {

            AbilityList abilityList = JsonUtility.FromJson<AbilityList>(jsonFile.text);
            unit.abilities.Clear();
            foreach (string abilityName in unit.abilityNames)
            {
                foreach (BasicAbility ability in abilityList.abilities)
                {
                    if (ability.name == abilityName)
                    {
                        unit.abilities.Add(ability);
                        break;  // Exit the inner loop once a match is found
                    }
                }
            }

            // Log abilities of the character to verify
            string parentObjectName = unit.gameObject.name;
            Debug.Log("Character Abilities for " + parentObjectName + ":");
            foreach (var ability in unit.abilities)
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

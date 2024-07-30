using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoader : MonoBehaviour
{
    public TextAsset jsonFile;
    public Unit characterAbilities;
    public BattleSystem battleSystem;

    void Start()
    {
        if (jsonFile != null)
        {

            AbilityList abilityList = JsonUtility.FromJson<AbilityList>(jsonFile.text);
            characterAbilities.abilities.Clear();
            foreach (string abilityName in characterAbilities.abilityNames)
            {
                foreach (BasicAbility ability in abilityList.abilities)
                {
                    if (ability.name == abilityName)
                    {
                        characterAbilities.abilities.Add(ability);
                        break;  // Exit the inner loop once a match is found
                    }
                }
            }

            // Log abilities of the character to verify
            string parentObjectName = characterAbilities.gameObject.name;
            Debug.Log("Character Abilities for " + parentObjectName + ":");
            foreach (var ability in characterAbilities.abilities)
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

        if (battleSystem == null)
        {
            battleSystem = FindObjectOfType<BattleSystem>();
        }

        // Start the SetupBattle coroutine
        StartCoroutine(battleSystem.SetupBattle());
    }
}

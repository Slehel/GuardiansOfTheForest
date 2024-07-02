using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoader : MonoBehaviour
{
    public TextAsset jsonFile;

    void Start()
    {
        if (jsonFile != null)
        {
            AbilityList abilityList = JsonUtility.FromJson<AbilityList>(jsonFile.text);
            foreach (BasicAbility ability in abilityList.abilities)
            {
                Debug.Log("Ability Name: " + ability.name);
                Debug.Log("Damage: " + ability.damage);
                Debug.Log("Cooldown: " + ability.cooldown);
                Debug.Log("Range: " + ability.range);
            }
        }
        else
        {
            Debug.LogError("JSON file not found!");
        }
    }
}


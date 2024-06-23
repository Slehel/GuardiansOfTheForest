using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterAbilities : MonoBehaviour
{
    public Ability[] abilities = new Ability[4];
    private AbilityManager abilityManager;

    private void Awake()
    {
        abilityManager = FindObjectOfType<AbilityManager>();
        foreach (var ability in abilities)
        {
            if (ability != null)
            {
                ability.Initialize(abilityManager);
            }
        }
    }

    public string GetAbilityName(int index)
    {
        if (index >= 0 && index < abilities.Length && abilities[index] != null)
        {
            return abilities[index].properties.abilityName;
        }
        return "Unknown";
    }
    public void UseAbility(int index)
    {

        if (abilities[index] != null)
        {
            
            abilities[index].Activate(gameObject);
        }
    }
}


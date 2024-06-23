using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/OffensiveAbility")]
public class OffensiveAbility : Ability
{
    public override void Activate(GameObject parent)
    {
        Debug.Log($"{abilityType} activated with damage: {properties.damage}");
    }

    /*public override string getAbilityName(GameObject parent, int index)
    {
        return properties.abilityName;
    }*/
}
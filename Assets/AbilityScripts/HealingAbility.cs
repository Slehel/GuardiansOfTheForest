using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/HealingAbility")]
public class HealingAbility : Ability
{
    public override void Activate(GameObject parent)
    {
        Debug.Log($"{abilityType} activated with heal: {properties.heal}");
    }
    /*public override string getAbilityName(GameObject parent,int index)
    {
        return properties.abilityName;
    }*/
}

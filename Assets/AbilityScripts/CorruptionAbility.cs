using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/CorruptionAbility")]
public class CorruptionAbility : Ability
{
    public override void Activate(GameObject parent)
    {
        Debug.Log($"{abilityType} activated with corruption damage: {properties.corruptionDamage}");
    }

    /*public override string getAbilityName(GameObject parent, int index)
    {
        return properties.abilityName;
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public AbilityType abilityType;
    public AbilityProperties properties;

    public void Initialize(AbilityManager abilityManager)
    {
        properties = abilityManager.GetAbilityProperties(abilityType);
    }

    //public abstract string getAbilityName(GameObject parent,int index);
    public abstract void Activate(GameObject parent);
}

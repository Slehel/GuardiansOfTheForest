using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<AbilityProperties> abilityPropertiesList;

    private Dictionary<AbilityType, AbilityProperties> abilityPropertiesDict;

    private void Awake()
    {
        abilityPropertiesDict = new Dictionary<AbilityType, AbilityProperties>();

        foreach (var properties in abilityPropertiesList)
        {
            abilityPropertiesDict.Add(properties.abilityType, properties);
        }
    }

    public AbilityProperties GetAbilityProperties(AbilityType abilityType)
    {
        if (abilityPropertiesDict.TryGetValue(abilityType, out var properties))
        {
            return properties;
        }

        return null;
    }
}
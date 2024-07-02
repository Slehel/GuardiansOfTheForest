using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAbility
{
    public string name;
    public int damage;
    public float cooldown;
    public float range;
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}


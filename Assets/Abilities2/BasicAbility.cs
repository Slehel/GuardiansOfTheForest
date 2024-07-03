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

    public void useFireball()
    {
        Debug.Log("Dealt 5 damge with Fireball");
    }
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}




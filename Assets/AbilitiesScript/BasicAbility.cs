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

    public void useAbility(Unit target)
    {
       if(name == "Fireball")
        {
            target.TakeDamage(damage);
            Debug.Log(name + " deals " + damage + " damage to " + target.unitName);
        }
        if (name == "Ice Blast")
        {
            target.TakeDamage(damage);
            Debug.Log(name + " deals " + damage + " damage to " + target.unitName);
        }
        if (name == "Lightning Strike")
        {
            target.TakeDamage(damage);
            Debug.Log(name + " deals " + damage + " damage to " + target.unitName);
        }
        if (name == "Heal")
        {
            target.TakeDamage(damage);
            Debug.Log(name + " deals " + damage + " damage to " + target.unitName);
        }
    }
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}




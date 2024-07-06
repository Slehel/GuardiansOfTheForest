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

    public void useAbility(BasicAbility ability)
    {
       if(ability.name == "Fireball")
        {
            Debug.Log("Fireball deals 5 damage");
        }
        if (ability.name == "Ice Blast")
        {
            Debug.Log("Ice Blast deals 5 damage");
        }
        if (ability.name == "Lightning Strike")
        {
            Debug.Log("Lightning Strike deals 5 damage");
        }
        if (ability.name == "Heal")
        {
            Debug.Log("Heal deals 5 damage");
        }
    }
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}




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
    public bool isHeal;
    public int stunChance;
    public int bleedChance;
    public int bleedDamage;
    public int poisonChance;
    public int poisonDamage;
    public int natureEffect;

    public int currentCooldown;

    public void UseAbility(Unit target)
    {
        if (isHeal)
        {
            target.Heal(damage);
            Debug.Log(name + " heals " + target.unitName + " for " + damage);
        }
        else
        {
            int effectiveDamage = Mathf.Max(1, damage - target.TotalDefense);
            bool killed = target.TakeDamage(effectiveDamage);
            Debug.Log(name + " deals " + effectiveDamage + " damage to " + target.unitName +
                      (target.TotalDefense > 0 ? $" ({target.TotalDefense} absorbed)" : ""));

            if (!killed && stunChance > 0 && Random.Range(0, 100) < stunChance)
            {
                target.isStunned = 1;
                Debug.Log(target.unitName + " is stunned!");
            }
        }
        currentCooldown = (int)cooldown;
    }

    public bool IsOnCooldown() => currentCooldown > 0;

    public void TickCooldown()
    {
        if (currentCooldown > 0) currentCooldown--;
    }
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}

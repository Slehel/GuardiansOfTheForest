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
    public int stunChance = 0;
    public int bleedChance = 0;
    public int bleedDamage = 0;
    public int poisonChance = 0;
    public int poisonDamage = 0;
    public int natureEffect = 0;

    // If false, ability fires immediately on the caster (no enemy click needed)
    public bool requiresTarget = true;

    // The actual ability logic — assigned in AbilityLoader.SetupAbilities()
    [System.NonSerialized]
    public System.Action<Unit> effect;

    public void useAbility(Unit target)
    {
        effect?.Invoke(target);
    }
}

[System.Serializable]
public class AbilityList
{
    public BasicAbility[] abilities;
}

using System.Collections.Generic;
using UnityEngine;

// Replaces JSON-based loading with hardcoded per-character ability setup.
public class AbilityLoader : MonoBehaviour
{
    public void LoadAbilities(Unit unit)
    {
        unit.abilities.Clear();
        string n = unit.unitName.ToLower();

        if (n.Contains("bear") || n.Contains("bearimus"))
            SetupBearimus(unit);
        else if (n.Contains("bunny") || n.Contains("lotus"))
            SetupLotus(unit);
        else
            SetupEnemy(unit);

        Debug.Log("Abilities loaded for " + unit.unitName + ": " + unit.abilities.Count);
    }

    // ── Bearimus Maximus (Bear Paladin) ───────────────────────────────────────

    void SetupBearimus(Unit unit)
    {
        // 1. Heavy Stomper — heavy single-target hit
        var heavyStomper = new BasicAbility
        {
            name = "Heavy Stomper",
            damage = unit.damage + 8,
            requiresTarget = true
        };
        heavyStomper.effect = (target) =>
        {
            target.TakeDamage(heavyStomper.damage, unit);
            Debug.Log("Heavy Stomper hits " + target.unitName + " for " + heavyStomper.damage);
        };

        // 2. Fear The Rage — reduces enemy outgoing damage by 30% next attack
        var fearTheRage = new BasicAbility
        {
            name = "Fear The Rage",
            damage = 0,
            requiresTarget = true
        };
        fearTheRage.effect = (target) =>
        {
            target.damageMultiplier = 0.7f;
            Debug.Log("Fear The Rage: " + target.unitName + " will deal 30% less damage next attack.");
        };

        // 3. Rush It Down — charge attack, deals damage
        var rushItDown = new BasicAbility
        {
            name = "Rush It Down",
            damage = unit.damage + 3,
            requiresTarget = true
        };
        rushItDown.effect = (target) =>
        {
            target.TakeDamage(rushItDown.damage, unit);
            Debug.Log("Rush It Down hits " + target.unitName + " for " + rushItDown.damage);
        };

        // 4. Get Behind Me! — Bearimus defends himself, protecting the team
        var getBehindMe = new BasicAbility
        {
            name = "Get Behind Me!",
            damage = 0,
            requiresTarget = false
        };
        getBehindMe.effect = (caster) =>
        {
            caster.isDefending = true;
            Debug.Log("Get Behind Me! " + caster.unitName + " takes a defensive stance!");
        };

        unit.abilities.AddRange(new List<BasicAbility> { heavyStomper, fearTheRage, rushItDown, getBehindMe });
    }

    // ── Lotus Bunnymur (Fighter Bunny) ────────────────────────────────────────

    void SetupLotus(Unit unit)
    {
        // 1. Steel and Stab — standard melee hit
        var steelAndStab = new BasicAbility
        {
            name = "Steel and Stab",
            damage = unit.damage,
            requiresTarget = true
        };
        steelAndStab.effect = (target) =>
        {
            target.TakeDamage(steelAndStab.damage, unit);
            Debug.Log("Steel and Stab hits " + target.unitName + " for " + steelAndStab.damage);
        };

        // 2. Feel My Teeth — damage + applies bleed (3 dmg/turn for 3 turns)
        var feelMyTeeth = new BasicAbility
        {
            name = "Feel My Teeth",
            damage = unit.damage - 2,
            bleedDamage = 3,
            requiresTarget = true
        };
        feelMyTeeth.effect = (target) =>
        {
            target.TakeDamage(feelMyTeeth.damage, unit);
            target.isBleeding = true;
            target.bleedDamage = 3;
            target.bleedDuration = 3;
            Debug.Log("Feel My Teeth: " + target.unitName + " is now bleeding!");
        };

        // 3. Lucky Feet — self speed buff (+5 speed next round)
        var luckyFeet = new BasicAbility
        {
            name = "Lucky Feet",
            damage = 0,
            requiresTarget = false
        };
        luckyFeet.effect = (caster) =>
        {
            caster.speed += 5;
            Debug.Log("Lucky Feet: " + caster.unitName + " feels light on their feet! Speed +" + 5);
        };

        // 4. Parry The Carry — defend + counter (reflects half damage back to attacker)
        var parryTheCarry = new BasicAbility
        {
            name = "Parry The Carry",
            damage = 0,
            requiresTarget = false
        };
        parryTheCarry.effect = (caster) =>
        {
            caster.isDefending = true;
            caster.isCountering = true;
            caster.counterDamage = unit.damage / 2;
            Debug.Log("Parry The Carry: " + caster.unitName + " is ready to counter!");
        };

        unit.abilities.AddRange(new List<BasicAbility> { steelAndStab, feelMyTeeth, luckyFeet, parryTheCarry });
    }

    // ── Enemies (shared ability set) ──────────────────────────────────────────

    void SetupEnemy(Unit unit)
    {
        // 1. Corrupt Strike — deals corruption damage
        var corruptStrike = new BasicAbility
        {
            name = "Corrupt Strike",
            damage = unit.corruptionDamage > 0 ? unit.corruptionDamage : unit.damage,
            requiresTarget = true
        };
        corruptStrike.effect = (target) =>
        {
            target.TakeDamage(corruptStrike.damage, unit);
            Debug.Log("Corrupt Strike hits " + target.unitName + " for " + corruptStrike.damage);
        };

        // 2. Rabid Bite — damage + wildness buildup on target
        var rabidBite = new BasicAbility
        {
            name = "Rabid Bite",
            damage = unit.damage,
            requiresTarget = true
        };
        rabidBite.effect = (target) =>
        {
            target.TakeDamage(rabidBite.damage, unit);
            target.wildness += 10;
            Debug.Log("Rabid Bite: " + target.unitName + " takes " + rabidBite.damage + " dmg, wildness +" + 10);
        };

        unit.abilities.AddRange(new List<BasicAbility> { corruptStrike, rabidBite });
    }
}

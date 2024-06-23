using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public string UnitClass;
    public Sprite CharacterPortrait;
    public int damage;
    public int maxHp;
    public int currentHp;
    public int corruptionDamage;
    public int NatureHeal;
    public int wildness;

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            return true;
        }
        else
        { 
            return false; 
        }
    }

}


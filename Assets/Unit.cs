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
    public Slider hpSlider;

    public string[] abilityNames = new string[4];
    public List<BasicAbility> abilities = new List<BasicAbility>();

    private BattleSystem battleSystem;

    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();

        
    }

    public void SetCharacterHpSlider()
    {
        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
    }
    void OnMouseDown()
    {
        if (battleSystem != null && battleSystem.state == BattleState.PLAYERTURN)
        {
            battleSystem.OnEnemyClicked(this);
        }
    }

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;
        hpSlider.value = currentHp;

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


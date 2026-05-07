using System;
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
    public int defense;
    public int speed;
    public int currentSpeed;
    public Boolean isPlayerCharacter;
    public int maxHp;
    public int currentHp;
    public int corruptionDamage;
    public int natureHeal;
    public int wildness;
    public int isStunned;
    public Slider hpSlider;
    public SpriteRenderer unitSpriteRenderer;
    private Coroutine highlightCoroutine;

    public string[] abilityNames = new string[4];
    public List<BasicAbility> abilities = new List<BasicAbility>();

    private BattleSystem battleSystem;


    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        unitSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    public void SetCharacterHpSlider()
    {
        hpSlider.maxValue = TotalMaxHp;
        hpSlider.value = currentHp;
    }
    void OnMouseDown()
    {
        if (battleSystem != null && battleSystem.state == BattleState.PLAYERTURN)
        {
            battleSystem.OnEnemyClicked(this);
        }
    }

    private HeroInventory Inventory => GetComponent<HeroInventory>();

    public int TotalDamage  => damage  + (Inventory != null ? Inventory.GetBonusDamage()  : 0);
    public int TotalDefense => defense + (Inventory != null ? Inventory.GetBonusDefense() : 0);
    public int TotalSpeed   => Mathf.Max(1, speed + (Inventory != null ? Inventory.GetBonusSpeed()  : 0));
    public int TotalMaxHp   => maxHp   + (Inventory != null ? Inventory.GetBonusMaxHp()   : 0);

    public bool IsDead => currentHp <= 0;

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;
        hpSlider.value = currentHp;
        return currentHp <= 0;
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, TotalMaxHp);
        hpSlider.value = currentHp;
    }

    public void HighlightTarget()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(TargetHighlightCoroutine());
    }

    public void StopHighlighting()//felesleges egyelore
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
            unitSpriteRenderer.color = Color.white; // Reset to original color
        }
    }

    private IEnumerator TargetHighlightCoroutine()
    {
        Color originalColor = unitSpriteRenderer.color;
        unitSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(2f); // Highlight duration

        unitSpriteRenderer.color = originalColor;
        highlightCoroutine = null;
    }

    public void LogUnitInfo()
    {
        string unitInfo = $"Unit Name: {unitName}\n" +
                          $"Unit Level: {unitLevel}\n" +
                          $"Unit Class: {UnitClass}\n" +
                          $"Damage: {damage}\n" +
                          $"Speed: {speed}\n" +
                          $"Current Speed: {currentSpeed}\n" +
                          $"Is Player Character: {isPlayerCharacter}\n" +
                          $"Max HP: {maxHp}\n" +
                          $"Current HP: {currentHp}\n" +
                          $"Corruption Damage: {corruptionDamage}\n" +
                          $"Nature Heal: {natureHeal}\n" +
                          $"Wildness: {wildness}\n" +
                          $"HP Slider Value: {(hpSlider != null ? hpSlider.value.ToString() : "None")}\n" +
                          $"Character Portrait: {(CharacterPortrait != null ? CharacterPortrait.name : "None")}\n" +
                          $"Sprite Renderer: {(unitSpriteRenderer != null ? unitSpriteRenderer.sprite.name : "None")}\n" +
                          $"Abilities: {string.Join(", ", abilityNames)}\n" +
                          $"Ability Details: {string.Join(", ", abilities.ConvertAll(ability => ability.name))}";

        Debug.Log(unitInfo);
    }
}



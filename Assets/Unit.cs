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
    public int speed;
    public int currentSpeed;
    public Boolean isPlayerCharacter;
    public int maxHp;
    public int currentHp;
    public int corruptionDamage;
    public int NatureHeal;
    public int wildness;
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
                          $"Nature Heal: {NatureHeal}\n" +
                          $"Wildness: {wildness}\n" +
                          $"HP Slider Value: {(hpSlider != null ? hpSlider.value.ToString() : "None")}\n" +
                          $"Character Portrait: {(CharacterPortrait != null ? CharacterPortrait.name : "None")}\n" +
                          $"Sprite Renderer: {(unitSpriteRenderer != null ? unitSpriteRenderer.sprite.name : "None")}\n" +
                          $"Abilities: {string.Join(", ", abilityNames)}\n" +
                          $"Ability Details: {string.Join(", ", abilities.ConvertAll(ability => ability.name))}";

        Debug.Log(unitInfo);
    }
}



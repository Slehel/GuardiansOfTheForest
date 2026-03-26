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
    public int natureHeal;
    public int wildness;
    public int isStunned;
    public Slider hpSlider;
    public SpriteRenderer unitSpriteRenderer;
    private Coroutine highlightCoroutine;

    public string[] abilityNames = new string[4];
    public List<BasicAbility> abilities = new List<BasicAbility>();

    // Ability states
    public bool isDefending = false;    // Halves next incoming hit
    public bool isCountering = false;   // Counterattacks after being hit while defending
    public int counterDamage = 0;       // How much the counter hits back for
    public bool isBleeding = false;     // Takes bleed damage at start of turn
    public int bleedDamage = 0;         // Damage per bleed tick
    public int bleedDuration = 0;       // Turns of bleed remaining
    public float damageMultiplier = 1f; // Multiplied against outgoing damage (Fear The Rage reduces this)
    public Unit protectedUnit = null;   // If set, attacks on protectedUnit redirect to this unit

    private BattleSystem battleSystem;

    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        unitSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        battleSystem?.ShowTooltip(unitName + "  |  " + Mathf.Max(0, currentHp) + " / " + maxHp + " HP");
    }

    void OnMouseExit()
    {
        battleSystem?.HideTooltip();
    }

    void UpdateHoverLabel()
    {
        // Refresh tooltip if currently hovering this unit
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

    // Returns true if the unit died. attacker is needed for counterattack.
    public bool TakeDamage(int incomingDamage, Unit attacker = null)
    {
        // Apply defend: halve damage and clear defend state
        if (isDefending)
        {
            incomingDamage = incomingDamage / 2;
            isDefending = false;

            // Counter: hit the attacker back
            if (isCountering && attacker != null)
            {
                attacker.TakeDamage(counterDamage);
                isCountering = false;
            }
        }

        currentHp -= incomingDamage;
        hpSlider.value = currentHp;
        UpdateHoverLabel();
        return currentHp <= 0;
    }

    // Called at the start of this unit's turn to tick bleed damage
    public bool ProcessBleed()
    {
        if (!isBleeding) return false;

        currentHp -= bleedDamage;
        hpSlider.value = currentHp;
        UpdateHoverLabel();
        bleedDuration--;

        if (bleedDuration <= 0)
        {
            isBleeding = false;
            bleedDamage = 0;
            bleedDuration = 0;
        }

        return currentHp <= 0;
    }

    public void HighlightTarget()
    {
        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);
        highlightCoroutine = StartCoroutine(TargetHighlightCoroutine());
    }

    public void StopHighlighting()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
            unitSpriteRenderer.color = Color.white;
        }
    }

    private IEnumerator TargetHighlightCoroutine()
    {
        Color originalColor = unitSpriteRenderer.color;
        unitSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(2f);
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
                          $"Abilities: {string.Join(", ", abilityNames)}\n" +
                          $"Ability Details: {string.Join(", ", abilities.ConvertAll(a => a.name))}";
        Debug.Log(unitInfo);
    }
}

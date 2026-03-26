using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    // Prefabs for player team characters
    public GameObject bearPrefab;
    public GameObject bunnyPrefab;
    // Prefabs for enemy team characters
    public GameObject evilEngineerPrefab;
    public GameObject evilPolicemanPrefab;

    public Transform playerBattleStation; // Parent for player characters
    public Transform enemyBattleStation;  // Parent for enemy characters

    public List<Unit> allUnits = new List<Unit>();
    public List<GameObject> playerTeam = new List<GameObject>();
    private List<GameObject> enemyTeam = new List<GameObject>();

    private Vector3[] playerPositions = new Vector3[]
    {
        new Vector3(1, -3, 0),
        new Vector3(-2.5f, -3.5f, 0)
    };

    private Vector3[] enemyPositions = new Vector3[]
    {
        new Vector3(3, -0.5f, 0),
        new Vector3(5.5f, -1, 0)
    };

    private Unit bearUnit;
    private Unit bunnyUnit;
    private Unit enemyEngineer1;
    private Unit enemyPolice2;

    public BattleHUDScript playerHUD;
    public BattleHUDScript enemyHUD;
    public AbilityLoader abilityLoader;
    public ResultScreen resultScreen;

    public TextMeshProUGUI NarratorText;
    public TextMeshProUGUI tooltipText;
    public AbilityButton[] abilityButtons;

    private BasicAbility selectedAbility;
    private Unit selectedUnit;
    public BattleState state;
    public int combatRound;
    public TurnOrder turnOrder;
    private int currentTurnIndex;
    private Unit lastPlayerCharacter;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        AssignTeams();
        InstantiateTeams();

        if (abilityLoader == null)
            Debug.LogError("AbilityLoader is not assigned!");
        abilityLoader.LoadAbilities(bearUnit);
        abilityLoader.LoadAbilities(bunnyUnit);
        Debug.Log("bunny abilities count: " + bunnyUnit.abilities.Count);

        yield return new WaitForSeconds(2f);

        NarratorText.text = "Your Crew is in danger! " + enemyEngineer1.unitName + " attacked them!";

        foreach (var unit in playerTeam)
            allUnits.Add(unit.GetComponent<Unit>());
        foreach (var unit in enemyTeam)
            allUnits.Add(unit.GetComponent<Unit>());

        turnOrder.MakeTurnOrder(allUnits.ToArray());

        bearUnit.SetCharacterHpSlider();
        bunnyUnit.SetCharacterHpSlider();
        enemyEngineer1.SetCharacterHpSlider();
        enemyPolice2.SetCharacterHpSlider();

        yield return new WaitForSeconds(2f);

        CombatTurn();
    }

    void SetupBattleUI(Unit unit)
    {
        playerHUD.SetPlayerHUD(unit);
        abilityLoader.LoadAbilities(unit);
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < unit.abilities.Count)
                abilityButtons[i].SetupButton(unit, unit.abilities[i], this);
            else
                abilityButtons[i].gameObject.SetActive(false);
        }
    }

    void AssignTeams()
    {
        playerTeam.Add(bearPrefab);
        playerTeam.Add(bunnyPrefab);

        enemyTeam.Add(evilEngineerPrefab);
        enemyTeam.Add(evilPolicemanPrefab);
    }

    void InstantiateTeams()
    {
        InstantiateTeam(playerTeam, playerBattleStation, playerPositions, true);
        InstantiateTeam(enemyTeam, enemyBattleStation, enemyPositions, false);
    }

    void InstantiateTeam(List<GameObject> team, Transform parent, Vector3[] positions, bool isPlayerTeam)
    {
        for (int i = 0; i < team.Count; i++)
        {
            GameObject prefab = team[i];
            GameObject characterObject = Instantiate(prefab, parent);
            characterObject.transform.localPosition = positions[i];

            Unit instantiatedUnit = characterObject.GetComponent<Unit>();

            if (isPlayerTeam)
            {
                if (prefab == bearPrefab)
                {
                    bearUnit = instantiatedUnit;
                    Debug.Log("Player unit assigned: " + bearUnit.unitName);
                }
                if (prefab == bunnyPrefab)
                {
                    bunnyUnit = instantiatedUnit;
                    Debug.Log("Player unit assigned: " + bunnyUnit.unitName);
                }
            }
            else
            {
                if (prefab == evilEngineerPrefab)
                {
                    enemyEngineer1 = instantiatedUnit;
                    Debug.Log("Enemy unit assigned: " + enemyEngineer1.unitName);
                }
                if (prefab == evilPolicemanPrefab)
                {
                    enemyPolice2 = instantiatedUnit;
                    Debug.Log("Enemy unit assigned: " + enemyPolice2.unitName);
                }
            }
        }
    }

    // Returns a random living player unit
    Unit GetRandomLivingPlayerUnit()
    {
        List<Unit> living = new List<Unit>();
        foreach (var go in playerTeam)
        {
            Unit u = go.GetComponent<Unit>();
            if (u == null) // use the instantiated unit refs instead
                continue;
            living.Add(u);
        }
        // Use the cached unit refs since playerTeam holds prefabs, not instances
        List<Unit> livingUnits = new List<Unit>();
        if (bearUnit != null && bearUnit.currentHp > 0) livingUnits.Add(bearUnit);
        if (bunnyUnit != null && bunnyUnit.currentHp > 0) livingUnits.Add(bunnyUnit);

        if (livingUnits.Count == 0) return null;
        return livingUnits[UnityEngine.Random.Range(0, livingUnits.Count)];
    }

    bool AllEnemiesDead()
    {
        if (enemyEngineer1 != null && enemyEngineer1.currentHp > 0) return false;
        if (enemyPolice2 != null && enemyPolice2.currentHp > 0) return false;
        return true;
    }

    bool AllPlayersDead()
    {
        if (bearUnit != null && bearUnit.currentHp > 0) return false;
        if (bunnyUnit != null && bunnyUnit.currentHp > 0) return false;
        return true;
    }

    IEnumerator EnemyTurn(Unit enemyUnit)
    {
        NarratorText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(2f);

        Unit target = GetRandomLivingPlayerUnit();
        if (target == null)
        {
            state = BattleState.LOST;
            EndBattle();
            yield break;
        }

        int finalDamage = Mathf.RoundToInt(enemyUnit.damage * enemyUnit.damageMultiplier);
        enemyUnit.damageMultiplier = 1f; // reset multiplier after use
        bool isDead = target.TakeDamage(finalDamage, enemyUnit);
        playerHUD.SetHP(target);
        NarratorText.text = "The attack is successful! " + target.unitName + " received " + finalDamage + " damage.";

        yield return new WaitForSeconds(2f);

        if (AllPlayersDead())
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
                StartNewRound();
            else
                CombatTurn();
        }
    }

    void StartNewRound()
    {
        turnOrder.ResetSpeeds();
        turnOrder.RollForTurnOrder();
        currentTurnIndex = 0;
        CombatTurn();
    }

    void CombatTurn()
    {
        Unit currentUnit = turnOrder.GetNextUnit(currentTurnIndex);

        // Skip dead units
        if (currentUnit.currentHp <= 0)
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
                StartNewRound();
            else
                CombatTurn();
            return;
        }

        // Process bleed at start of unit's turn — use coroutine so text is readable
        if (currentUnit.isBleeding)
        {
            StartCoroutine(ProcessBleedThenContinue(currentUnit));
            return;
        }

        StartTurn(currentUnit);
    }

    IEnumerator ProcessBleedThenContinue(Unit currentUnit)
    {
        int bleedTick = currentUnit.bleedDamage;
        bool diedFromBleed = currentUnit.ProcessBleed();

        NarratorText.text = "[BLEED] " + currentUnit.unitName + " is bleeding! Takes " + bleedTick + " damage. ("
            + Mathf.Max(0, currentUnit.currentHp) + " / " + currentUnit.maxHp + " HP remaining)";

        yield return new WaitForSeconds(1.5f);

        if (diedFromBleed)
        {
            NarratorText.text = currentUnit.unitName + " has bled out!";
            yield return new WaitForSeconds(1f);
            if (AllEnemiesDead()) { state = BattleState.WON; EndBattle(); yield break; }
            if (AllPlayersDead()) { state = BattleState.LOST; EndBattle(); yield break; }
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count) StartNewRound();
            else CombatTurn();
            yield break;
        }

        StartTurn(currentUnit);
    }

    void StartTurn(Unit currentUnit)
    {
        if (currentUnit.isPlayerCharacter)
        {
            state = BattleState.PLAYERTURN;
            lastPlayerCharacter = currentUnit;
            SetupBattleUI(currentUnit);
            NarratorText.text = currentUnit.unitName + "'s turn! Choose an ability!";
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn(currentUnit));
        }
    }

    public void OnAbilityButtonClicked(BasicAbility ability)
    {
        if (state != BattleState.PLAYERTURN) return;

        selectedAbility = ability;

        if (!ability.requiresTarget)
        {
            // Self-cast: fire immediately on the caster, no target click needed
            StartCoroutine(PlayerUseAbility(lastPlayerCharacter));
        }
        else
        {
            NarratorText.text = "Select a target for " + ability.name;
        }
    }

    public void OnEnemyClicked(Unit enemyUnit)
    {
        if (state != BattleState.PLAYERTURN || selectedAbility == null) return;

        enemyUnit.HighlightTarget();
        StartCoroutine(PlayerUseAbility(enemyUnit));
    }

    IEnumerator PlayerUseAbility(Unit targetUnit)
    {
        selectedAbility.useAbility(targetUnit);

        NarratorText.text = lastPlayerCharacter.unitName + " used " + selectedAbility.name + " on " + targetUnit.unitName + "!";
        selectedAbility = null;

        yield return new WaitForSeconds(1f);

        if (AllEnemiesDead())
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
                StartNewRound();
            else
                CombatTurn();
        }
    }

    public void ShowTooltip(string text)
    {
        if (tooltipText != null)
        {
            tooltipText.text = text;
            tooltipText.gameObject.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltipText != null)
            tooltipText.gameObject.SetActive(false);
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            NarratorText.text = "Victory! The forest is safe!";
            if (resultScreen != null) resultScreen.ShowVictory();
        }
        else if (state == BattleState.LOST)
        {
            NarratorText.text = "Defeat... The corruption wins.";
            if (resultScreen != null) resultScreen.ShowDefeat();
        }
    }
}

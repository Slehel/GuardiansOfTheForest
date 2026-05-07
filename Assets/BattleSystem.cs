using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    // Prefabs for player team characters
    public GameObject bearPrefab;
    public GameObject bunnyPrefab;
    public GameObject foxPrefab;
    public GameObject wolfPrefab;
    // Prefabs for enemy team characters
    public GameObject evilEngineerPrefab;
    public GameObject evilPolicemanPrefab;
    public GameObject evilFirefighterPrefab;
    public GameObject evilDoctorPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public List<Unit> allUnits = new List<Unit>();
    public List<GameObject> playerTeam = new List<GameObject>();
    private List<GameObject> enemyTeam = new List<GameObject>();

    private Vector3[] playerPositions = new Vector3[]
    {
        new Vector3(1, -3, 0),
        new Vector3(-2.5f, -3.5f, 0),
        new Vector3(-5, -3.75f, 0),
        new Vector3(-7.5f, -3, 0)
    };

    private Vector3[] enemyPositions = new Vector3[]
    {
        new Vector3(3, -0.5f, 0),
        new Vector3(5.5f, -1, 0),
        new Vector3(8, -1, 0),
        new Vector3(10.5f, -1, 0)
    };

    private Unit bearUnit;
    private Unit foxUnit;
    private Unit wolfUnit;
    private Unit bunnyUnit;
    private Unit enemyEngineer1;
    private Unit enemyPolice2;
    private Unit enemyFirefighter3;
    private Unit enemyDoctor4;

    public BattleHUDScript playerHUD;
    public BattleHUDScript enemyHUD;
    public AbilityLoader abilityLoader;

    public TextMeshProUGUI NarratorText;
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
        abilityLoader.LoadAbilities(foxUnit);
        abilityLoader.LoadAbilities(wolfUnit);
        abilityLoader.LoadAbilities(bunnyUnit);

        // Apply Skill Forge upgrades from homebase
        ApplyAbilityUpgrades(bearUnit);
        ApplyAbilityUpgrades(foxUnit);
        ApplyAbilityUpgrades(wolfUnit);
        ApplyAbilityUpgrades(bunnyUnit);

        yield return new WaitForSeconds(2f);

        NarratorText.text = " Your Crew is in danger! " + enemyEngineer1.unitName + " crew attacked them!";

        foreach (var unit in playerTeam)
            allUnits.Add(unit.GetComponent<Unit>());
        foreach (var unit in enemyTeam)
            allUnits.Add(unit.GetComponent<Unit>());

        turnOrder.MakeTurnOrder(allUnits.ToArray());

        bearUnit.SetCharacterHpSlider();
        foxUnit.SetCharacterHpSlider();
        wolfUnit.SetCharacterHpSlider();
        bunnyUnit.SetCharacterHpSlider();
        enemyEngineer1.SetCharacterHpSlider();
        enemyPolice2.SetCharacterHpSlider();
        enemyFirefighter3.SetCharacterHpSlider();
        enemyDoctor4.SetCharacterHpSlider();

        yield return new WaitForSeconds(2f);

        CombatTurn();
    }

    void SetupBattleUI(Unit unit)
    {
        playerHUD.SetPlayerHUD(unit);

        // Only reload abilities if not yet loaded (preserves cooldown state)
        if (unit.abilities.Count == 0)
            abilityLoader.LoadAbilities(unit);

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < unit.abilities.Count)
                abilityButtons[i].SetupButton(unit, unit.abilities[i], this);
            else
                Debug.Log($"Ability button {i} exceeds ability count for {unit.unitName}.");
        }
    }

    void ApplyAbilityUpgrades(Unit unit)
    {
        if (GameManager.Instance == null) return;
        foreach (var ability in unit.abilities)
        {
            int level = GameManager.Instance.GetAbilityUpgradeLevel(ability.name);
            if (level <= 0) continue;
            ability.damage       = Mathf.RoundToInt(ability.damage * (1f + level * 0.25f));
            ability.cooldown     = Mathf.Max(1f, ability.cooldown - level * 0.5f);
            ability.stunChance  += level * 10;
            ability.poisonChance += level * 10;
            ability.poisonDamage += level * 3;
        }
    }

    void AssignTeams()
    {
        playerTeam.Add(bearPrefab);
        playerTeam.Add(bunnyPrefab);
        playerTeam.Add(foxPrefab);
        playerTeam.Add(wolfPrefab);

        enemyTeam.Add(evilEngineerPrefab);
        enemyTeam.Add(evilPolicemanPrefab);
        enemyTeam.Add(evilFirefighterPrefab);
        enemyTeam.Add(evilDoctorPrefab);
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
                if (prefab == bearPrefab) bearUnit = instantiatedUnit;
                else if (prefab == bunnyPrefab) bunnyUnit = instantiatedUnit;
                else if (prefab == foxPrefab) foxUnit = instantiatedUnit;
                else if (prefab == wolfPrefab) wolfUnit = instantiatedUnit;
            }
            else
            {
                if (prefab == evilEngineerPrefab) enemyEngineer1 = instantiatedUnit;
                else if (prefab == evilPolicemanPrefab) enemyPolice2 = instantiatedUnit;
                else if (prefab == evilFirefighterPrefab) enemyFirefighter3 = instantiatedUnit;
                else if (prefab == evilDoctorPrefab) enemyDoctor4 = instantiatedUnit;
            }
        }
    }

    private List<Unit> GetLivingPlayerUnits() =>
        allUnits.FindAll(u => u.isPlayerCharacter && !u.IsDead);

    private List<Unit> GetLivingEnemyUnits() =>
        allUnits.FindAll(u => !u.isPlayerCharacter && !u.IsDead);

    IEnumerator EnemyTurn(Unit enemyUnit)
    {
        // Handle stun: skip turn if stunned
        if (enemyUnit.isStunned > 0)
        {
            enemyUnit.isStunned--;
            NarratorText.text = enemyUnit.unitName + " is stunned and loses their turn!";
            yield return new WaitForSeconds(1.5f);
            AdvanceTurn();
            yield break;
        }

        NarratorText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(2f);

        // Pick a random living player to attack
        List<Unit> livingPlayers = GetLivingPlayerUnits();
        if (livingPlayers.Count == 0)
        {
            state = BattleState.LOST;
            EndBattle();
            yield break;
        }

        Unit target = livingPlayers[UnityEngine.Random.Range(0, livingPlayers.Count)];
        int rawDamage = enemyUnit.damage;
        int effectiveDamage = Mathf.Max(1, rawDamage - target.TotalDefense);
        bool isDead = target.TakeDamage(effectiveDamage);

        playerHUD.SetHP(target);
        string blockText = target.TotalDefense > 0 ? $" ({target.TotalDefense} blocked)" : "";
        NarratorText.text = enemyUnit.unitName + " attacks " + target.unitName +
                            " for " + effectiveDamage + " damage!" + blockText;

        yield return new WaitForSeconds(2f);

        if (GetLivingPlayerUnits().Count == 0)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            AdvanceTurn();
        }
    }

    void StartNewRound()
    {
        combatRound++;
        turnOrder.ResetSpeeds();
        turnOrder.RollForTurnOrder();
        currentTurnIndex = 0;
        CombatTurn();
    }

    void CombatTurn()
    {
        // Skip dead units
        int safetyCounter = 0;
        while (safetyCounter < allUnits.Count)
        {
            Unit candidate = turnOrder.GetNextUnit(currentTurnIndex);
            if (!candidate.IsDead) break;
            currentTurnIndex++;
            safetyCounter++;
        }

        Unit currentUnit = turnOrder.GetNextUnit(currentTurnIndex);

        if (currentUnit.isPlayerCharacter)
        {
            // Handle stun: skip turn if stunned
            if (currentUnit.isStunned > 0)
            {
                currentUnit.isStunned--;
                NarratorText.text = currentUnit.unitName + " is stunned and loses their turn!";
                TickCooldownsForUnit(currentUnit);
                AdvanceTurn();
                return;
            }

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

    void AdvanceTurn()
    {
        currentTurnIndex++;
        if (currentTurnIndex >= allUnits.Count)
            StartNewRound();
        else
            CombatTurn();
    }

    void TickCooldownsForUnit(Unit unit)
    {
        foreach (var ability in unit.abilities)
            ability.TickCooldown();
    }

    public void OnAbilityButtonClicked(BasicAbility ability)
    {
        if (state != BattleState.PLAYERTURN) return;

        if (ability.IsOnCooldown())
        {
            NarratorText.text = ability.name + " is on cooldown for " + ability.currentCooldown + " more turn(s)!";
            return;
        }

        selectedAbility = ability;
        NarratorText.text = "Select a target for " + ability.name;
    }

    public void OnEnemyClicked(Unit enemyUnit)
    {
        if (state != BattleState.PLAYERTURN || selectedAbility == null) return;

        // Heal abilities target allies, not enemies — ignore enemy clicks
        if (selectedAbility.isHeal)
        {
            NarratorText.text = selectedAbility.name + " can only target allies!";
            return;
        }

        enemyUnit.HighlightTarget();
        StartCoroutine(PlayerUseAbility(enemyUnit));
    }

    public void OnAllyClicked(Unit allyUnit)
    {
        if (state != BattleState.PLAYERTURN || selectedAbility == null) return;
        if (!selectedAbility.isHeal) return;

        StartCoroutine(PlayerUseAbility(allyUnit));
    }

    IEnumerator PlayerUseAbility(Unit targetUnit)
    {
        selectedAbility.UseAbility(targetUnit);

        NarratorText.text = lastPlayerCharacter.unitName + " used " + selectedAbility.name + " on " + targetUnit.unitName;

        TickCooldownsForUnit(lastPlayerCharacter);
        selectedAbility = null;

        yield return new WaitForSeconds(1f);

        if (GetLivingEnemyUnits().Count == 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            AdvanceTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            NarratorText.text = "You won the battle!";
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LastBattleWon = true;
                GameManager.Instance.MarkCurrentRoomCleared();
            }
        }
        else if (state == BattleState.LOST)
        {
            NarratorText.text = "You were defeated...";
            if (GameManager.Instance != null)
                GameManager.Instance.LastBattleWon = false;
        }

        StartCoroutine(ReturnToDungeon());
    }

    IEnumerator ReturnToDungeon()
    {
        yield return new WaitForSeconds(2.5f);

        if (GameManager.Instance != null)
            GameManager.Instance.SavePartyHp(allUnits);

        // Return to dungeon map if it exists, otherwise stay (standalone battle test)
        if (GameManager.Instance != null && GameManager.Instance.HasDungeon())
            SceneManager.LoadScene("DungeonScene");
    }
}

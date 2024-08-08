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
    public GameObject foxPrefab;
    public GameObject wolfPrefab;
    // Prefabs for enemy team characters
    public GameObject evilEngineerPrefab;
    public GameObject evilPolicemanPrefab;
    public GameObject evilFirefighterPrefab;
    public GameObject evilDoctorPrefab;

    public Transform playerBattleStation;// Parent for player characters
    public Transform enemyBattleStation; // Parent for enemy characters

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
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;

        // Start the SetupBattle coroutine
        StartCoroutine(SetupBattle());

        //StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        AssignTeams();
        InstantiateTeams();

        //Load Abilities
        abilityLoader.LoadAbilities(bearUnit);
        abilityLoader.LoadAbilities(foxUnit);
        abilityLoader.LoadAbilities(wolfUnit);
        abilityLoader.LoadAbilities(bunnyUnit);
        Debug.Log("bunny abilities count"+ bunnyUnit.abilities.Count);
        yield return new WaitForSeconds(2f);

        NarratorText.text = " Your Crew is in danger! " + enemyEngineer1.unitName + " crew attacked them!";

        // Adding all Gameobjects Unit to the allUnits list
        foreach (var unit in playerTeam)
        {
            allUnits.Add(unit.GetComponent<Unit>());
        }
        foreach (var unit in enemyTeam)
        {
            allUnits.Add(unit.GetComponent<Unit>());
        }

        turnOrder.MakeTurnOrder(allUnits.ToArray());

        

        //Setting Sliders for characters
        bearUnit.SetCharacterHpSlider();
        foxUnit.SetCharacterHpSlider();
        wolfUnit.SetCharacterHpSlider();
        bunnyUnit.SetCharacterHpSlider();
        enemyEngineer1.SetCharacterHpSlider();
        enemyPolice2.SetCharacterHpSlider();
        enemyFirefighter3.SetCharacterHpSlider();
        enemyDoctor4.SetCharacterHpSlider();

        

        

        //Setup HUD and Player buttons
       /* playerHUD.SetPlayerHUD(bearUnit);
        //Setup Ability Buttons for playerCharacters
        for (int i = 0; i < abilityButtons.Length; i++)
            if (i < bearUnit.abilities.Count)
            {
                abilityButtons[i].SetupButton(bearUnit, bearUnit.abilities[i], this);
            }
            else
            {
                Debug.Log($"Ability button at index {i} exceeds the number of abilities on the character.");
            }*/
        yield return new WaitForSeconds(2f);

        //Start the Combat Rounds
        CombatTurn();
    }

    void SetupBattleUI(Unit unit)
    {
        Debug.Log("SETUP UI");
        //unit.LogUnitInfo();
        //Setup HUD and Player buttons
        playerHUD.SetPlayerHUD(unit);
        abilityLoader.LoadAbilities(unit);// NA SZOVAL AMIG A TEAMEKET NEM RENDEZTED EL RENDESEN, addig kell ez a loadabilities minden ui betolteskor, mert valszeg elcsuszik a prefab meg a unitokkal
        //Debug.Log(unit.abilities[1]); nincs meg az abilities
        //Setup Ability Buttons for playerCharacters
        for (int i = 0; i < abilityButtons.Length; i++)
            if (i < unit.abilities.Count)
            {
                abilityButtons[i].SetupButton(unit, unit.abilities[i], this);
            }
            else
            {
                Debug.Log($"Ability button at index {i} exceeds the number of abilities on the character." + "unit abilities count is" + unit.abilities.Count + "  ");
            }
    }

    void AssignTeams()
    {
        // Hardcoded assignment of prefabs to teams
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
                if (prefab == bearPrefab)
                {
                    bearUnit = instantiatedUnit;
                    //Debug.Log("Player unit assigned1: " + bearUnit.unitName);
                }
                if (prefab == bunnyPrefab)
                {
                    bunnyUnit = instantiatedUnit;
                }
                if (prefab == foxPrefab)
                {
                    foxUnit = instantiatedUnit;
                }
                if (prefab == wolfPrefab)
                {
                    wolfUnit = instantiatedUnit;
                }
            }
            else
            {
                if (prefab == evilEngineerPrefab)
                {
                    enemyEngineer1 = instantiatedUnit;
                    //Debug.Log("Enemy unit assigned1: " + enemyEngineer1.unitName);
                }
                if (prefab == evilPolicemanPrefab)
                {
                    enemyPolice2 = instantiatedUnit;
                }
                if (prefab == evilFirefighterPrefab)
                {
                    enemyFirefighter3 = instantiatedUnit;
                }
                if (prefab == evilDoctorPrefab)
                {
                    enemyDoctor4 = instantiatedUnit;
                }
            }
        }
    }

    IEnumerator EnemyTurn(Unit enemyUnit)
    {
        NarratorText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(2f);

        bool isDead = bearUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetBearHP(bearUnit);
        NarratorText.text = "The attack is successful " + bearUnit.unitName + " received " + enemyUnit.damage + " damage";

        //playerHUD.SetHP(bearUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
            {
                StartNewRound();
            }
            else
            {
                CombatTurn();
            }
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
        //currentUnit.LogUnitInfo();
        if (currentUnit.isPlayerCharacter)
        {
            state = BattleState.PLAYERTURN;
            lastPlayerCharacter= currentUnit;
            SetupBattleUI(currentUnit);
            NarratorText.text = currentUnit.unitName + " turn starts now! Choose an ability!";
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn(currentUnit));
        }
    }

    public void OnAbilityButtonClicked(BasicAbility ability)
    {
        if (state == BattleState.PLAYERTURN)
        {
            selectedAbility = ability;
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

        NarratorText.text = lastPlayerCharacter.unitName + " used " + selectedAbility.name + " on " + targetUnit.unitName;
        selectedAbility = null;

      //  targetUnit.StopHighlighting();

        yield return new WaitForSeconds(1f);

        if (targetUnit.currentHp <= 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
            {
                StartNewRound();
            }
            else
            {
                CombatTurn();
            }
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            NarratorText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            NarratorText.text = "You were defeated.";
        }
    }
}

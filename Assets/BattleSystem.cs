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

    private List<GameObject> playerTeam = new List<GameObject>();
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

    private Unit playerUnit;
    private Unit enemyUnit;

    public BattleHUDScript playerHUD;
    public BattleHUDScript enemyHUD;    


    public TextMeshProUGUI NarratorText;

    public CharacterAbilities bearCharacter;
    public AbilityButton[] abilityButtons;

    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        AssignTeams();
        InstantiateTeams();

        NarratorText.text = " Your Crew is in danger! " + enemyUnit.unitName + " attacked them!";
        
        playerHUD.SetPlayerHUD(playerUnit);
        enemyHUD.SetEnemyHUD(enemyUnit);

        //Setup Ability Buttons for playerCharacters
        for (int i = 0; i < abilityButtons.Length; i++)
            if (i < bearCharacter.abilities.Count)
            {
                abilityButtons[i].SetupButton(bearCharacter, bearCharacter.abilities[i]);
            }
            else
            {
                Debug.Log($"Ability button at index {i} exceeds the number of abilities on the character.");
            }
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
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
                    playerUnit = instantiatedUnit;
                    Debug.Log("Player unit assigned: " + playerUnit.unitName);
                }
            }
            else
            {
                if (prefab == evilEngineerPrefab)
                {
                    enemyUnit = instantiatedUnit;
                    Debug.Log("Enemy unit assigned: " + enemyUnit.unitName);
                }
            }
        }
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetEnemySliderHP(enemyUnit.currentHp);
        NarratorText.text = "The attack is successful " + enemyUnit.unitName + " received " + playerUnit.damage + " damage";
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        NarratorText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(2f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        NarratorText.text = "The attack is successful " + playerUnit.unitName + " received " + enemyUnit.damage + " damage";

        //playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
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

    void PlayerTurn()
    {
        NarratorText.text = playerUnit.unitName + " turn starts now! Choose an ability!";
    }

    public void onAttackButton()
    {
        if (state != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }

}

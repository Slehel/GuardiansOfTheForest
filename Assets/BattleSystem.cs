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

    Unit playerUnit;
    Unit enemyUnit;

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
        
        GameObject playerGo = Instantiate(bearPrefab, playerBattleStation);
        playerUnit = playerGo.GetComponent<Unit>();
        GameObject enemyGo = Instantiate(evilEngineerPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

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

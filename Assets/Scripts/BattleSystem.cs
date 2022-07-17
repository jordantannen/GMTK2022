using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Transform diceSpot;
	public GameObject diePrefab;
	Dice die; 

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;
	public GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		GameObject diceGO = Instantiate(diePrefab, diceSpot);
		die = diceGO.GetComponent<Dice>();

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack(AbilityBase move)
	{

		
		bool isDead = enemyUnit.TakeDamage(move.Power);

		enemyHUD.SetHP(enemyUnit.currentHP);

		if (move.Power > 0)
		{
			dialogueText.text = "The attack is successful!";
		}
		

		yield return new WaitForSeconds(3f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} 
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " is rolling!";

		StartCoroutine(HandleRollEnemy());

		yield return new WaitForSeconds(1f);

		//bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(2f);

	}

	IEnumerator EnemyAttack(AbilityBase move)
	{
		bool isDead = playerUnit.TakeDamage(move.Power);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "The enemy attack is successful!";

		yield return new WaitForSeconds(3f);

		if (isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		}
	}

	IEnumerator HandleRollEnemy()
	{
		yield return StartCoroutine(die.RollTheDice(enemyUnit.moves));
		yield return new WaitForSeconds(1f);

		int side = die.finalSide;
		
		StartCoroutine(EnemyAttack(enemyUnit.moves[die.finalSide]));
		StartCoroutine(EnemyHeal(enemyUnit.moves[die.finalSide]));
		yield return new WaitForSeconds(1.2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator EnemyHeal(AbilityBase move)
	{
		enemyUnit.Heal(move.Heal);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The enemy feels renewed strength!";

		yield return new WaitForSeconds(2f);
	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			manager.GetComponent<GameManager>().Invoke("GetNewAbility", 1.5f);
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
			Invoke("Restart", 2f);
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
		die.rollable = true;
	}

	IEnumerator PlayerHeal(AbilityBase move)
	{
		playerUnit.Heal(move.Heal);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(3f);
	}

	public void OnRollButton()
    {
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(HandleRoll());
		
	}

	IEnumerator HandleRoll()
    {
		yield return StartCoroutine(die.RollTheDice(GameManager.learnedMoves));
		yield return new WaitForSeconds(1f);

		int side = die.finalSide;
		int randint = Random.Range(0, 100);
		if (randint <= GameManager.learnedMoves[die.finalSide].Accuracy)
		{
			StartCoroutine(PlayerAttack(GameManager.learnedMoves[die.finalSide]));
			StartCoroutine(PlayerHeal(GameManager.learnedMoves[die.finalSide]));
		}
		else
		{
			dialogueText.text = "The attack missed!";
		}
		yield return new WaitForSeconds(2f);
		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
		yield break;
	}	

	public void Restart()
	{
		SceneManager.LoadScene(0);
	}
}

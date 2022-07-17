using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
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

	IEnumerator EnemyAttack()
	{
		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "The enemy attack is successful!";

		yield return new WaitForSeconds(3f);

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

	IEnumerator HandleRollEnemy()
	{
		yield return StartCoroutine(die.RollTheDice());
		yield return new WaitForSeconds(1f);

		int side = die.finalSide;
		if (side == 1)
			StartCoroutine(EnemyAttack());
		else
			StartCoroutine(EnemyHeal());
	}

	IEnumerator EnemyHeal()
	{
		enemyUnit.Heal(5);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The enemy feels renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
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
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
		die.rollable = true;
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnRollButton()
    {
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(HandleRoll());
		
	}

	IEnumerator HandleRoll()
    {
		yield return StartCoroutine(die.RollTheDice());
		yield return new WaitForSeconds(1f);

		int side = die.finalSide;
		StartCoroutine(PlayerAttack(GameManager.learnedMoves[die.finalSide]));
		yield break;
		if (side == 1)
			StartCoroutine(PlayerAttack(GameManager.learnedMoves[die.finalSide]));
		else
			StartCoroutine(PlayerHeal());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		//StartCoroutine(die.RollTheDice());
		int side = die.finalSide;
		StartCoroutine(PlayerAttack(GameManager.learnedMoves[die.finalSide]));
		return;
		if (side == 1)
			StartCoroutine(PlayerAttack(GameManager.learnedMoves[die.finalSide]));
		else
			StartCoroutine(PlayerHeal());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

}

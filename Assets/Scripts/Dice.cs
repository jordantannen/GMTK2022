using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour {

    // Array of dice sides sprites to load from Resources folder
    public Sprite[] diceSides;

    public int finalSide = 1;
    public bool rollable = false;

    // Reference to sprite renderer to change sprites
    private SpriteRenderer rend;

	// Use this for initialization
	private void Start () {

        // Assign Renderer component
        rend = GetComponent<SpriteRenderer>();

        // Load dice sides sprites to array from DiceSides subfolder of Resources folder
        //diceSides = Resources.LoadAll<Sprite>("/Images/Dice/");
	}
	
    // If you left click over the dice then RollTheDice coroutine is started
    private void OnMouseDown()
    {
        if (rollable)
        {
            StartCoroutine("RollTheDice");
        }
    }

    // Coroutine that rolls the dice
    public IEnumerator RollTheDice(AbilityBase[] moves)
    {
        rollable = false;
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide = 0;

        //finalSide = Random.Range(0, diceSides.Length) + 1;

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 20; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, moves.Length);

            // Set sprite to upper face of dice from array according to random value
            rend.sprite = moves[randomDiceSide].Sprite;

            // Pause before next itteration
            yield return new WaitForSeconds(0.05f);
        }

        // Assigning final side so you can use this value later in your game
        // for player movement for example
        finalSide = randomDiceSide;
        
        // Show final dice value in Console
        Debug.Log(finalSide);
        Debug.Log(rollable);
    }
}

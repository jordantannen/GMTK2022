using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    
    public AbilityBase[] moves;
    public Canvas abilitySelect;
    static public AbilityBase[] learnedMoves;
    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
        Destroy(gameObject);
        return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        learnedMoves = new AbilityBase[2];
        learnedMoves[0] = moves[0];
        learnedMoves[1] = moves[1];
        learnedMoves[2] = moves[2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNewAbility()
    {
        AbilityBase[] options = new AbilityBase[3];
        for (int i = 0; i < 3; i++)
        {
            options[i] = moves[Random.Range(0, moves.Length)];
        }
        abilitySelect.GetComponent<AbilitySelect>().ShowOptions(options);
    }

    static public void LearnMove(AbilityBase move)
    {
        AbilityBase[] tmp = new AbilityBase[learnedMoves.Length + 1];
        learnedMoves.CopyTo(tmp, 0);
        tmp[tmp.Length - 1] = move;
        learnedMoves = tmp;
    }
}

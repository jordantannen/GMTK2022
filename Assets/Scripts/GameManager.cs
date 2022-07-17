using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    
    public AbilityBase[] moves;
    public Canvas abilitySelect;
    // Start is called before the first frame update
    void Start()
    {
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
}

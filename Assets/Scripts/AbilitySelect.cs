using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySelect : MonoBehaviour
{

    public Button[] buttons;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(buttons.Length);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOptions(AbilityBase[] options)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < options.Length; i++)
        {
            buttons[i].GetComponentInChildren<TMP_Text>().text = options[i].name;
            AbilityBase tmp = options[i];
            int tmp2 = i;
            buttons[i].onClick.AddListener(delegate {Select(tmp);});
        }
    }

    public void Select(AbilityBase move)
    {
        gameObject.SetActive(false);
        Unit.learnedMoves.Add(move);
    }
}

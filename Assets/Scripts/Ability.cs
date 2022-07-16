using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public float power;
    public float activeTime;

    public virtual void Activate()
    {

    }
}

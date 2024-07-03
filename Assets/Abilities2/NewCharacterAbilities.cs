using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacterAbilities : MonoBehaviour
{
    public string[] abilityNames = new string[4];
    public List<BasicAbility> abilities = new List<BasicAbility>();

    private void Awake()
    {
        // Any initialization if needed
    }
}

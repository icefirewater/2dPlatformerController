using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLookForPlayerData", menuName = "Data/State Data/Look For Player State")]

public class D_LookForPlayer : ScriptableObject
{
    public int amountOfTurn = 2;
    public float timeBetweenTurn = 0.75f;
}

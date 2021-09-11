using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMoveStateData", menuName = "Data/State Data/Move State")]


public class D_MoveState : ScriptableObject       // Scriptable object is a Data Container to save large amount of data independent of Class Intances
{
    public float movementSpeed = 3f;
   
}

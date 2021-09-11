using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/EntityData/BaseData")]

public class D_Entity : ScriptableObject                    // we need to create Data objects in Unity & this is something ScriptableObject allows us to do
{
    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.45f;

    public LayerMask whatIsGround; 
}

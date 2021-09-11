using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]

public class D_Entity : ScriptableObject                    // we need to create Data objects in Unity & this is something ScriptableObject allows us to do
{

    public float maxHealth = 30.0f;

    public float damageHopSpeed = 3f;

    public float wallCheckDistance = 0.2f;
    public float groundCheckRadius = 0.3f;
    public float ledgeCheckDistance = 0.45f;

    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;

    public float stunResistance = 3f;
    public float stunRecoveryTime = 2f;

    public float closeRangeActionDistance = 1.0f;

    public GameObject hitParticle;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
}

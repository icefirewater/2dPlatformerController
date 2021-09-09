using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMeleeAttackData", menuName = "Data/State Data/Melee Attack")]

public class D_MeleeAttack : ScriptableObject
{
    public float attackRadius = 0.5f;
    public float attackDamage = 10.0f; 

    public LayerMask whatIsPlayer;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newStunStateData", menuName = "Data/State Data/Stun State")]
public class D_StunState : ScriptableObject
{
    public float stunTime = 0.5f;

    public float stunKnockbackTime = 0.2f;

    public float stunKnockbackSpeed = 15f;
    
    public Vector2 stunKnockBackAngle;
}

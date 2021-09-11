using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAssetMenu", menuName = "Data/State Data/Charged State" )]

public class D_ChargeState : ScriptableObject
{
    public float chargeSpeed = 6.0f;
    public float chargeTime = 0.5f;
}

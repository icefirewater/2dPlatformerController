using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled; 
    [SerializeField] private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField] private Transform attackHitBoxPos;                       //store a ref to GO we will create as a child to Player & allow HitBox where we want
    [SerializeField] private LayerMask whatIsDamagable;                       //this will tell what all objects need to be detected as damagable & what not. 

    private bool gotInput, isAttacking, isFirstAttack; 
    private float lastInputTime = Mathf.NegativeInfinity;               //will always attack from the start of the game

    //Ref
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()                   //detect combat related input from player
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //attack1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;                               //alternate between 2  attack animations
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time > (lastInputTime + inputTimer))
        {
            //wait for new input
            gotInput = false;
        }
    }

    private void CheckHitBox()                                           // detect all damagable object in range and damage that
    {
        Collider2D[] detectedObject = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attack1Radius, whatIsDamagable);

        foreach (Collider2D col in detectedObject)
        {
            col.transform.parent.SendMessage("Damage", attack1Damage);             //used to call a specific function on a script on a object without knowing which script it is
            //Instantiate Hit Particle
        }
    }    

    private void FinishAttack1()                                       // it will be  called after Attack Animation & let our script know that it is done
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void OnDrawGizmos()                                      // use to draw  our HitBox so we can see it in our scene
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attack1Radius);
    }
}

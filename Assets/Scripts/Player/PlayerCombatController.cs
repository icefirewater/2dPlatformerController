using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;                              // control whether we want our character to Attack/Hit other GO or not 
    [SerializeField] private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField] private Transform attackHitBoxPos;                       // store a ref to GO we will create as a child to Player & allow HitBox where we want
    [SerializeField] private LayerMask whatIsDamagable;                       // this will tell what all objects need to be detected as damagable & what not. 

    private bool gotInput, isAttacking, isFirstAttack;                        // hold the input from player so if player hit just before he is able to hit, the player will still hit once he is able to hit  

    private float lastInputTime = Mathf.NegativeInfinity;                     // will store last time player had attacked & mathf.negativeInfinity will always attack from the start of the game
    private AttackDetails attackDetails;
    //Ref
    private Animator anim;                                                    // hold the ref to Animator component
    private PlayerController player;
    private PlayerStats playerStats;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);                             // combat is enabled
        player = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()                                            // detect any combat related input from player
    {
        if (Input.GetMouseButtonDown(0))                                       // this is true when left mouse button is pressed
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()                                                // making the attack happen when gets input 
    {
        if (gotInput)
        {
            //attack1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;                               // alternate between 2  attack animations
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time > (lastInputTime + inputTimer))                         // inputTimer is how long to hold the input for
        {
            //wait for new input
            gotInput = false;
        }
    }

    private void CheckHitBox()                                                // detect all damagable object in range and damage that
    {
        Collider2D[] detectedObject = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attack1Radius, whatIsDamagable);

        attackDetails.damageAmount = attack1Damage;
        attackDetails.position = transform.position;

        foreach (Collider2D col in detectedObject)
        {
            col.transform.parent.SendMessage("Damage", attackDetails);         // sendMessage is used to call a specific function on a script on a object without knowing which script it is
            //Instantiate Hit Particle
        }
    }

    private void FinishAttack1()                                               // it will be called after Attack Animation & let our script know that it is done
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void Damage(AttackDetails _attackDetails)
    {
        if (!player.GetDashStatus())
        {
            int direction;

            playerStats.DecreaseHealth(attackDetails.damageAmount);                                           // damage player here using attackDeatails[0]

            if (attackDetails.position.x < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            player.Knockback(direction);
        }
    }

    private void OnDrawGizmos()                                               // use to draw our HitBox so we can see it in our scene
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attack1Radius);
    }
}

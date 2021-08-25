using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour   
{
    private enum State
    {
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField] private Transform 
        groundCheck, 
        wallCheck,
        touchDamageCheck;                 // this will be a GO as a child of our Enemy that will be the center of touch damage area

    [SerializeField] private LayerMask 
        whatIsGround,
        whatIsPlayer;

    [SerializeField] private float 
        groundCheckDistance, 
        wallCheckDistance, 
        movementSpeed, 
        maxHealth, 
        knockbackDuration,
        touchDamageCooldown,
        touchDamage,                     // define how much damage it gives on touch
        touchDamageWidth,                // bounding box width so if player touches box its get damage
        touchDamageHeight;

    [SerializeField] private Vector2 knockbackSpeed;

    [SerializeField] private GameObject 
        hitParticle, 
        deathChunkParticle, 
        deathBloodParticle;  

    private bool 
        groundDetected, 
        wallDetected;
    
    private int 
        facingDirection = 1, 
        damageDirection;

    private float[] attackDetails = new float[2];

    private float
        currentHealth,
        knockbackStartTime,
        lastTouchDamageTime;


    private Vector2
        movement,
        touchDamageBotLeft,
        touchDamageTopRight;

    private GameObject aliveGO;
    private Rigidbody2D aliveRB;
    private Animator aliveAnim;

    private void Start()
    {
        aliveGO = transform.Find("Alive").gameObject;
        aliveRB = aliveGO.GetComponent<Rigidbody2D>();
        aliveAnim = aliveGO.GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            
            case State.Knockback:
                UpdateKnockBackState();
                break;

            case State.Dead:
                UpdateDeadState();
                break;
        }
    }
    

    // ----------------------------Walking State-----------------------------------

    private void EnterMovingState() 
    {

    }

    private void UpdateMovingState()    
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);

        CheckTouchDamage();

        if (!groundDetected || wallDetected)
        {

            Flip();

        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }
    }

    private void ExitMovingState()  
    {

    }


    // ---------------------------KnockBack State-----------------------------------

    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRB.velocity = movement;

        aliveAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockBackState()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }


    // ----------------------------Dead State--------------------------------------

    private void EnterDeadState()   
    {
        // Spawn Blood Chunks
        Instantiate(deathChunkParticle, aliveGO.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, aliveGO.transform.position, deathBloodParticle.transform.rotation); 

        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }


    // ------------------------Other Functions-------------------------------------

    private void SwitchState(State _state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;

            case State.Knockback:
                ExitKnockbackState();
                break;

            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (_state)
        {
            case State.Moving:
                EnterMovingState();
                break;

            case State.Knockback:
                EnterKnockbackState();
                break;

            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = _state;
    }

    private void Damage(float[] _attackDetails)
    {
        currentHealth -= _attackDetails[0];

        Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if(_attackDetails[1] > aliveGO.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        // Hit Particle

        if(currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
            touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));
            
            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if(hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = aliveGO.transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector2 scale = aliveGO.transform.localScale;
        scale.x *= -1.0f;
        aliveGO.transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));


        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2)); 
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2)); 
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2)); 
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}

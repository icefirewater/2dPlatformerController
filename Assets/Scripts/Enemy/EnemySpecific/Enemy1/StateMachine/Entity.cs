using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Entity Class is the Base Class for Enemies so all Enemies are going to Inherit Entity class

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject aliveGO { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }

    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;
    //[SerializeField] private Transform MaxAgroRangeCheck;

    private Vector2 velocityWorkspace;

    public virtual void Start()
    {
        facingDirection = 1;
        aliveGO = transform.Find("Alive").gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = aliveGO.GetComponent<Animator>();
        atsm = aliveGO.GetComponent<AnimationToStateMachine>();
        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float _velocity) 
    {
        velocityWorkspace.Set(_velocity * facingDirection, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    { 
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckPlayerInMinAgroRange() 
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
        
    }
    public virtual bool CheckPlayerInMaxAgroRange()  
    { 
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual void Flip()
    {
        facingDirection *= -1;
        //Vector2 scale = aliveGO.transform.localScale;
        //scale.x *= -1.0f;
        //aliveGO.transform.localScale = scale;
        aliveGO.transform.Rotate(0.0f, -180.0f, 0.0f);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * facingDirection * entityData.ledgeCheckDistance));
       
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.2f);
         
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.2f);
    }
}

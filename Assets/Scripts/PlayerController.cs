using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;   
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform pfAirJumpIcon;
    [SerializeField] private int airJumpCountMax;


    private int airJumpCount;

    private float horizontal;
    private float groundCheckRadius = 0.3f;
    private float wallCheckDistance = 0.4f;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool vertical;
    //private bool isTouchingWall;

    private List<GameObject> airJumpIconGameObjects;

    private Rigidbody2D rb;                                        // Reference to Rigidbody2D component
    private Animator anim;                                         // Ref to Animator Component

    void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();               //rb = GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        SetAirJumpCountMax(airJumpCountMax);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckInputDirection();
        HorizontalAnimation();
        HorizontalMovement();
        VerticalAnimation();
        VerticalMovement();
        CheckSurroundings();
        IsGrounded();
    }

    void FixedUpdate()
    {
    }

    private void CheckInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");              // Horizontal Movement Input (values between -1 - 0 - 1)
        vertical = Input.GetButtonDown("Jump");                   // Vertical Movement Input          
    }


    private void HorizontalAnimation() => anim.SetFloat("Speed", Mathf.Abs(horizontal));
    private void HorizontalMovement() => rb.velocity = new Vector2(movementSpeed * horizontal, rb.velocity.y);


    private void VerticalAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        //    anim.SetFloat("jumpVelocity", Mathf.Abs(rb.velocity.y));     // for Jump BlendTree 
    }
    private void VerticalMovement()
    {
        if (vertical)
        {
            CheckIfCanJump();
        }
    }

    private void CheckInputDirection()
    {
        if (isFacingRight & horizontal < 0)
        {
            Flip();
        }
        else if (!isFacingRight && horizontal > 0)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;                      // if isFacingRight is true then it will be false 
        Vector2 scale = transform.localScale;
        scale.x *= -1f;                                      //transform.Rotate(0.0f, 180.0f, 0.0f);
        transform.localScale = scale;
    }

    private void IsGrounded()
    {
        if (isGrounded)
        {
            ResetAirJumpCount();
        }
    }


    private void CheckIfCanJump()
    {
        //Test for Single Jump 
        if (isGrounded)
        {
            Jump();
            ResetAirJumpCount();
        }

        //Test for Double Jump 
        else if (airJumpCount < airJumpCountMax)
        {
            Jump();
            AirJumpSpend();
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        //isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
    }
    private void ResetAirJumpCount()
    {
        if (airJumpCount > 0)
        {
            airJumpCount = 0;
            for (int i = 0; i < airJumpIconGameObjects.Count; i++)
            {
                airJumpIconGameObjects[i].SetActive(true);
            }
        }
    }
    private void AirJumpSpend()
    {
        airJumpCount++;
        for (int i = 0; i < airJumpCount; i++)
        {
            airJumpIconGameObjects[i].SetActive(false);
        }
    }
    private void SetAirJumpCountMax(int airJumpCountMax)
    {
        this.airJumpCountMax = airJumpCountMax;
        airJumpIconGameObjects = new List<GameObject>();
        for (int i = 0; i < airJumpCountMax; i++)
        {
            Transform airJumpIconTransform = Instantiate(pfAirJumpIcon, transform);
            airJumpIconTransform.localPosition = new Vector2(-0.2f * i, -1.3f);
            airJumpIconGameObjects.Add(airJumpIconTransform.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
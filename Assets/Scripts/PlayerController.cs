using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;

    private int airJumpCount;
    private int airJumpCountMax;

    private float horizontal;
    private float groundCheckRadius = 0.3f;

    private bool isFacingRight = true;
    private bool isGrounded;
    //private bool canDoubleJump;
    private bool vertical;

    private Rigidbody2D rb;                                        // Reference to Rigidbody2D component
    private Animator anim;                                         // Ref to Animator Component

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();               //rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        HorizontalAnimation();
        CheckInputDirection();
        VerticalAnimation();
        airJumpCountMax = 1;
    }

    void FixedUpdate()
    {
        HorizontalMovement();
        VerticalMovement();
        CheckSurroundings();
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
    //    anim.SetFloat("jumpVelocity", Mathf.Abs(rb.velocity.y));
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
    
    private void CheckSurroundings() => isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    private void CheckIfCanJump()
    { 
        //Test for Single Jump 
        if (isGrounded) 
        {
            Jump();
        //    canDoubleJump = true;
            airJumpCount = 0;
        }
        //Test for Double Jump
        //else if (canDoubleJump)
        else if (airJumpCount < airJumpCountMax)
        {
            Jump();
          //  canDoubleJump = false;
            airJumpCount++;
        }
        
    }
    private void Jump()
    {
            rb.velocity = Vector2.up * jumpForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
} 
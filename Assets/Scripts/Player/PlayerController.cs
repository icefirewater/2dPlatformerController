using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float airDragMultiplier = 0.95f;
    [SerializeField] private float turnTimerSet = 0.1f;
    [SerializeField] private float wallJumpTimerSet = 0.5f;
    [SerializeField] private int airJumpCountMax;
    [SerializeField] private Transform pfAirJumpIcon;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Vector2 wallJumpDirection;           // to store a vector that determines a direction at which we jump off the wall. So when we are steeper or lower it adjusts that
    [SerializeField] private Vector2 knockbackSpeed;

    [SerializeField] private float xWallHopForce;
    [SerializeField] private float yWallHopForce;
    [SerializeField] private float wallHopTime;
    [SerializeField] private float wallJumpForce;

    [SerializeField] private float ledgeClimbXOffset1 = 0f;
    [SerializeField] private float ledgeClimbYOffset1 = 0f;
    [SerializeField] private float ledgeClimbXOffset2 = 0f;
    [SerializeField] private float ledgeClimbYOffset2 = 0f;

    [SerializeField] private float knockbackDuration;

    [SerializeField] private float dashTime;                     // how long will dash take place
    [SerializeField] private float dashSpeed;                    // speed of dash
    [SerializeField] private float distanceBetweenImages;        // distance between dash image while dashing
    [SerializeField] private float dashCooldown;                 // how long we need to wait before dashing again
                                                                    
    private int airJumpCount;                                       
    private int facingDirection = 1;                             // with movement we do input direction to do the math but for Jumping off wall we do not need any X-Axis Input into the player 
    private int lastWallJumpDirection;                           
    
    private float horizontal;                                       
    private float turnTimer;
    private float wallJumpTimer;
    private float groundCheckRadius = 0.3f;                         
    private float wallCheckDistance = 0.35f;                        
                                                                    
    private float dashTimeLeft;                                  // how longer dash should be happening
    private float lastImageXPos;                                 // keep track of last X Co-Ordinate when we placed AfterImage
    private float lastDash = -100f;                              // Last time we started to dash & will be used to check cool down. -100 so we can dash as soon we start the game 
    private float knockbackStartTime;
    
    private bool isFacingRight = true;                              
    private bool isGrounded;                                        
    private bool vertical;                                          
    private bool canJump;                                           
    private bool isTouchingWall;                                    
    private bool isWallSliding;                                     
    private bool wallHop;                                           
    private bool isTouchingLedge;                                   
    private bool canClimbLedge = false;                             
    private bool ledgeDetected;                                     
    private bool dash;                                              
    private bool isDashing;                                      // isDashing or not
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool knockback;

    private Vector2 direction = Vector2.right;
    private Vector2 ledgePosBot;                                 // to store the position of raycast from where it was cast when Ledge was detected
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;


    private List<GameObject> airJumpIconGameObjects;

    private Rigidbody2D rb;                                      // Reference to Rigidbody2D component
    private Animator anim;                                       // Ref to Animator Component


    void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        SetAirJumpCountMax(airJumpCountMax);
        wallJumpDirection.Normalize();                           // it just gonna make the Vector itself 1 and that way when we multiply to the force we gonna add will be Force itself
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
        Dash();
        IsGrounded();
        CheckSurroundings();
        WallSliding();
        WallHop();
        CheckLedgeClimb();
        CheckDash();
        CheckKnockback();
    }

    #region Input
    private void CheckInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");              // Horizontal Movement Input (values between -1 - 0 - 1)
        vertical = Input.GetButtonDown("Jump");                   // Vertical Movement Input
        dash = Input.GetButtonDown("Dash");

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && horizontal != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;  
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
    }

    #endregion

    #region Movement&Animation
    private void HorizontalMovement()
    {
        if (!isGrounded && !isWallSliding && horizontal == 0 && !knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove && !knockback)
        {
            rb.velocity = new Vector2(movementSpeed * horizontal, rb.velocity.y);
        }
    }
    private void HorizontalAnimation() => anim.SetFloat("Speed", Mathf.Abs(horizontal));
    private void VerticalMovement()
    {
        if (vertical)
        {
            CheckIfCanJump();
            Jump();
        }
    }
    private void VerticalAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
        //    anim.SetFloat("jumpVelocity", Mathf.Abs(rb.velocity.y));     // for Jump BlendTree 
    }

    #endregion

    #region Dash
    private void Dash()
    {
        if (dash)                                              //if dash button pressed then look for cool down is finished
        {
            if (Time.time > (lastDash + dashCooldown))         //if cool down has passed then try to attemp dash 
            {
                AttemptToDash();                             
            }
        }
    }
    private void AttemptToDash()                              
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPos = transform.position.x;
    }

    private void CheckDash()                                 //it will be responsible for setting the Dash velocity & whether it should be dashing or should be stopped
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);           //Set Player velocity to Dash velocity
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)     //check enough distance have passed to place another AfterImage
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }
            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    #endregion

    #region Flipping
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
        if (!isWallSliding && canFlip && !isDashing && !ledgeDetected && !knockback)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;                      // if isFacingRight is true then it will be false 
            Vector2 scale = transform.localScale;
            scale.x *= -1f;                                      //transform.Rotate(0.0f, 180.0f, 0.0f);
            transform.localScale = scale;
            direction = -direction;
        }
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    public void DisableFlip()   
    {
        canFlip = false;
    }
    #endregion

    #region Jump

    private void CheckIfCanJump()
    {
        //Test for Single Jump 
        //if (isGrounded && canJump && !isWallSliding && !ledgeDetected)
        //{
        //    Jump();
        //}

        //Test for Double Jump 
        //if ((airJumpCount < airJumpCountMax) && !ledgeDetected)
        //{
        //    jumpForce /= 1.5f;
        //    Jump();
        //    AirJumpSpend();                                          //increase air jump count
        //    jumpForce *= 1.5f;
        //}

        if (airJumpCount == airJumpCountMax)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && horizontal == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, -0.01f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && canJump && !isWallSliding)
        {
            rb.velocity = Vector2.up * jumpForce;                      //same as rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        else if ((isWallSliding || isTouchingLedge) && horizontal != 0 && canJump)
        {
            isWallSliding = false;

            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * horizontal, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }

        else if ((airJumpCount < airJumpCountMax) && !ledgeDetected)
        {
            jumpForce /= 1.5f;                                       //Reducing the JumpForce for Double Jump
            //GroundJump
            rb.velocity = Vector2.up * jumpForce;
            AirJumpSpend();                                          //increase air jump count
            jumpForce *= 1.5f;
        }
    }

    #endregion

    #region WallSliding
    private void CheckIsWallSlding()
    {
        if (!isGrounded && isTouchingWall && horizontal == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void WallSliding()
    {
        CheckIsWallSlding();
        if (isWallSliding)
        {
            //if (rb.velocity.y < -wallSlideSpeed)
            //{
            //    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            //}
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            ResetAirJumpCount();
        }
    }

    #endregion

    #region WallHop
    private void WallHop()
    {
        CheckWallHop();
        if (wallHop)
        {
            rb.velocity = new Vector2(xWallHopForce * -(facingDirection), yWallHopForce);
        }
    }

    private void CheckWallHop()
    {
        if (isTouchingWall && vertical && horizontal == facingDirection && !ledgeDetected)
        {
            isWallSliding = false;
            wallHop = true;
            Invoke("SetWallHopToFalse", wallHopTime);
        }
    }
    private void SetWallHopToFalse() => wallHop = false;

    #endregion

    #region LedgeClimb
    private void CheckLedgeClimb()                               //Responsible for LedgeClimb to happen when player detect ledge
    {
        if (ledgeDetected && !canClimbLedge && rb.velocity.y < 0)
        {
            canClimbLedge = true;
            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            canFlip = false;
            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)                                    
        {
            airJumpIconGameObjects[(airJumpIconGameObjects.Count - 1)].SetActive(false);
            transform.position = ledgePos1;                   //if player finds the ledge we have to hold its pos to ledgePos1
        }

    }

    public void FinishLedgeClimb()                           // calling this function with unity events after ledgeClimb animation is finished. 
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        anim.SetBool("canClimbLedge", canClimbLedge);
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        airJumpIconGameObjects[(airJumpIconGameObjects.Count - 1)].SetActive(true);
    }
    #endregion

    #region Raycasts
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);  // transform.right
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, wallCheckDistance, whatIsGround);

        if (!isGrounded && isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;                   // to store the position as soon as the ledge is detected
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y));
    }
    #endregion

    #region JumpIcon
    private void IsGrounded()
    {
        if (isGrounded)
        {
            canJump = true;
            ResetAirJumpCount();
        }
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
    private void SetAirJumpCountMax(int _airJumpCountMax)
    {
        airJumpCountMax = _airJumpCountMax;
        airJumpIconGameObjects = new List<GameObject>();
        for (int i = 0; i < _airJumpCountMax; i++)
        {
            Transform airJumpIconTransform = Instantiate(pfAirJumpIcon, transform);
            airJumpIconTransform.localPosition = new Vector2(-0.2f * i, -1.3f);
            airJumpIconGameObjects.Add(airJumpIconTransform.gameObject);
        }
    }
    #endregion

    #region Knockback
    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
    #endregion

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }
}
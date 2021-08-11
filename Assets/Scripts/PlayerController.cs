using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private int airJumpCountMax;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform pfAirJumpIcon;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float xWallJumpForce;
    [SerializeField] private float yWallJumpForce;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float airDragMultiplier;

    private int airJumpCount;

    private float horizontal;
    private float groundCheckRadius = 0.3f;
    private float wallCheckDistance = 0.2f;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool vertical;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool isWallSliding;
    private bool wallJump;
    private bool canClimbLedge;
    private bool ledgeDetected = false;

    private Vector2 direction = Vector2.right;
    private Vector2 ledgePosBot;                                    // to store the position of raycast from where it was cast when Ledge was detected
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;


    private List<GameObject> airJumpIconGameObjects;

    private Rigidbody2D rb;                                        // Reference to Rigidbody2D component
    private Animator anim;                                         // Ref to Animator Component


    void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
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
        IsGrounded();
        CheckSurroundings();
        WallSliding();
        WallJump();
        CheckLedgeClimb();
    }

    #region Input
    private void CheckInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");              // Horizontal Movement Input (values between -1 - 0 - 1)
        vertical = Input.GetButtonDown("Jump");                   // Vertical Movement Input          
    }

    #endregion

    #region Movement&Animation
    private void HorizontalMovement()
    {
        if (!isGrounded && !isWallSliding && horizontal == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else
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
        }
    }
    private void VerticalAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
        //    anim.SetFloat("jumpVelocity", Mathf.Abs(rb.velocity.y));     // for Jump BlendTree 
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
        if (!isWallSliding)
        {
            isFacingRight = !isFacingRight;                      // if isFacingRight is true then it will be false 
            Vector2 scale = transform.localScale;
            scale.x *= -1f;                                      //transform.Rotate(0.0f, 180.0f, 0.0f);
            transform.localScale = scale;
            direction = -direction;
        }
    }
    #endregion

    #region Jump

    private void CheckIfCanJump()
    {
        //Test for Single Jump 
        if (isGrounded && !ledgeDetected)
        {
            Jump();
        }

        //Test for Double Jump 
        else if (airJumpCount < airJumpCountMax)
        {
            jumpForce /= 1.5f;
            Jump();
            AirJumpSpend();
            jumpForce *= 1.5f;
        }
    }
    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
    }

    #endregion

    #region WallSliding
    private void CheckIsWallSlding()
    {
        //if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        if (isTouchingWall && !isGrounded && horizontal != 0 && !canClimbLedge)
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

    #region WallJump
    private void CheckWallJump()
    {
        if (isWallSliding && vertical && !ledgeDetected)
        {
            wallJump = true;
            Invoke("SetWallJumpToFalse", wallJumpTime);
        }
    }
    private void SetWallJumpToFalse() => wallJump = false;

    private void WallJump()
    {
        CheckWallJump();
        if (wallJump)
        {
            rb.velocity = new Vector2(xWallJumpForce * -(horizontal), yWallJumpForce);
        }
    }

    #endregion

    #region LedgeClimb
    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge && rb.velocity.y < 0 )
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

            anim.SetBool("canClimbLedge", canClimbLedge);
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }

    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        ledgeDetected = false;
        transform.position = ledgePos2;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }
    #endregion

    #region Raycasts
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);  // transform.right
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, wallCheckDistance, whatIsGround);

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        //Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y));
    }
    #endregion

    #region JumpIcon
    private void IsGrounded()
    {
        if (isGrounded)
        {
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
}
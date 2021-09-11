using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    [SerializeField] private float maxHealth, knockBackSpeedX, knockBackSpeedY, knockBackDuration, knockBackDeathSpeedX, knockBackDeathSpeedY, deathTorque;
    [SerializeField] private bool applyKnockBack;
    [SerializeField] private GameObject pfHitParticles;

    private float currentHealth, knockbackStart;
    private int playerFacingDirection;
    private bool playerOnLeft, knockBack;

//    private PlayerController player;                                            // will hold ref to the script attached to Player & will be used to get direction of the player is facing
    private GameObject aliveGO, brokenTopGO, brokenBotGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBot;
    private Animator aliveAnim;


    private void Start()
    {
        currentHealth = maxHealth;                                              // initializes the current health to max health 
 //       player = GameObject.Find("Player").GetComponent<PlayerController>();    // looks through the hierarchy & returns the 1st GO it finds that has name Player
        
        aliveGO = transform.Find("Alive").gameObject;                           // looks for the child GO with the name Alive thiss script is attached to
        brokenBotGO = transform.Find("BrokenTop").gameObject;
        brokenTopGO = transform.Find("BrokenBottom").gameObject;

        // now we got GO & with that we can get other references
        aliveAnim = aliveGO.GetComponent<Animator>();
        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBot = brokenBotGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);                                                // enables the GO
        brokenTopGO.SetActive(false);                                           // disables the GO
        brokenBotGO.SetActive(false);
    }
    
    private void Update()
    {
        CheckKnockBack();
    }

    private void Damage(AttackDetails _attackDetails)
    {
        currentHealth -= _attackDetails.damageAmount; //_details[0];

        if(_attackDetails.position.x < aliveGO.transform.position.x)
        {
            playerFacingDirection = 1;
        }
        else
        {
            playerFacingDirection = -1;
        }

        PlayerPosition();                                                       // gets the position of the player

        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("damage");
        Instantiate(pfHitParticles, aliveGO.transform.position, Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f)));
        KnockBack();

        Die();
    }

    private void PlayerPosition()
    {
        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }
    }

    private void KnockBack()
    {
        if (applyKnockBack && currentHealth > 0.0f)
        {
        knockBack = true;
        knockbackStart = Time.time;
        rbAlive.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
        }
    }

    private void CheckKnockBack()                     // this will stop KnockBack after certain amt of Time is Passed.
    {
        if(Time.time > (knockbackStart + knockBackDuration) && knockBack)
        {
            knockBack = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);
        }
    }
    
    private void Die()
    {
        if (currentHealth <= 0)
        {
            aliveGO.SetActive(false);
            brokenTopGO.SetActive(true);
            brokenBotGO.SetActive(true);

            // as we have a moving Alive GO when it gets knockback so we need to set the position of BrokenTop GO & BrokenBot Go to that of Alive GO
            brokenTopGO.transform.position = aliveGO.transform.position;
            brokenBotGO.transform.position = aliveGO.transform.position;

            rbBrokenBot.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
            rbBrokenTop.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
            rbBrokenBot.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);              // this will cause the broken Top to rotate once it breaks
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterEffectSprite : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.1f;                           // how long GO should be active for
    [SerializeField] private float alphaSet = 0.8f;                    // to decrease the alpha overtime
    

    //Ref
    private Transform player;                                          //Need ref to player to get position and rotation 
    private SpriteRenderer sR;                                         
    private SpriteRenderer playerSR;
    private Color color;

    
    private float timeActivated;                                                 //how long it has been activated
    private float alpha;
    private float alphaMultiplier = 0.85f;

    private void OnEnable()
    {
        sR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time; 
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1, 1, 1, alpha);
        sR.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);                  //Instead of destroying GO we add it AddToPool()
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterEffectSprite : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.1f;                      //5. how long GO should be active for
    [SerializeField] private float alphaSet = 0.8f;                        //8. Need to set the Alpha when we enable GO
    [SerializeField] private float alphaDecay;                             //9. Need to decrease Alpha overtime

    //Ref                                                                       
    private Transform player;                                              //1. Need ref to player to get its position and rotation 
                                                                                
    private SpriteRenderer sR;                                             //2. Ref to the Sprite Renderer(SR) on this AfterImage GameObject(GO)
    private SpriteRenderer playerSR;                                       //3. Ref to the Player GO's SR so we can have its current sprite
                                                                                
    private Color color;                                                   //4. Need to decrease Alpha over time which means we need to change the Color setting 
                                                                                
    private float timeActivated;                                           //6. how long it has been activated
    private float alpha;                                                   //7. need to know what Alpha currently is  
                                                                         
                                                                           
    private void OnEnable()                                                //10. Its like Start(), Awake() funtions except that it gets called every time we enable the GO 
    {                                                                            
        sR = GetComponent<SpriteRenderer>();                               //11. we get the ref of the SR component of this GO
        player = GameObject.FindGameObjectWithTag("Player").transform;     //12. Cache ref of Player GO
        playerSR = player.GetComponent<SpriteRenderer>();                  //13. Cache ref og Playr's SR

        alpha = alphaSet;                                                  //14. Set Alpla to alphaSet
        sR.sprite = playerSR.sprite;                                       //15. Set this GO sprite to that of current Sprite of Player to 
        transform.position = player.position;                              //16. Set this GO position to Player GO position
        transform.rotation = player.rotation;                              //17. Set this GO rotation to Player GO rotation
        timeActivated = Time.time;                                          
    }

    private void Update()
    {
        alpha -= alphaDecay * Time.deltaTime;                                          // Decrease the alpha
        color = new Color(1, 1, 1, alpha);                                 // create new Color with new Alpha value
        sR.color = color;                                                  // Set the new color to this SR

        if(Time.time >= (timeActivated + activeTime))                      // Check if this AfterImage has been activated long enough
        {
            // add back to Pool
            PlayerAfterImagePool.Instance.AddToPool(gameObject);           // Instead of destroying GO we add it the POOL (AddToPool function)
        }
    }
}

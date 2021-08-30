using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject pfAfterImage;                           // this is use to store ref of AfterImage Prefab

    private Queue<GameObject> availableObjects = new Queue<GameObject>();       // used to store all the objects made which are not not currently active

    public static PlayerAfterImagePool Instance { get; private set; }           // Basic Singleton with public get & private set that will be used to access the script from other scripts

    private void Awake()
    {
        Instance = this;                                                        // Set the ref to this script
        GrowPool();
    }

    private void GrowPool()                                       // Function to create more GO for the pool
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(pfAfterImage);        // var is dataType which tells the compiler to figure out what type it should be when it compiles. So when we declare & set it equal to GO then compiler will knowthat it is GO
            instanceToAdd.transform.SetParent(transform);         // makes the GO created a child of the GO this script is attach to
            AddToPool(instanceToAdd);                             // pass the instance to AddToPool()
        }
    }

    public void AddToPool(GameObject _instance)                   // should be public so that we can Call from other Scripts via Singleton
    {
        _instance.SetActive(false);
        availableObjects.Enqueue(_instance);                      // add the GO to the Queue(availableObjects)
    }

    public GameObject GetFromPool()                               // should be public so we can call from other scripts & return type GO so will return GO 
    {
        if (availableObjects.Count == 0)                          // if we trying to get AfterImage GO to spawn and NONE is available then we will make some more
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();               //this will take the object from the Queue
        instance.SetActive(true);                                //OnEnable function gets called in PlayerAfterImageSprite Script
        return instance;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject pfAfterImage;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();               //All objects made which are not not currently active

    public static PlayerAfterImagePool Instance { get; private set; }                 // Basic Singleton with public get & private set

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()                                       //to create more GO for the pool
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(pfAfterImage);        //var is dataType which tells the compiler what it should be when it compiles
            instanceToAdd.transform.SetParent(transform);         // makes the GO created a child of the GO this script is attach to
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject _instance)                   // should be public so that we can Call from other
    {
        _instance.SetActive(false);
        availableObjects.Enqueue(_instance);                          //add the GO to the Queue with 
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)                          // if we trying to get AfterImage to spawn and NONE is available then we will make some more
        {
            GrowPool();
        }

        var _instance = availableObjects.Dequeue();               //this will take the object from the Queue
        _instance.SetActive(true);                                //OnEnable function gets called in PlayerAfterImage sprite Script
        return _instance;
    }
}

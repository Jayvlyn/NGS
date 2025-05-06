using System.Collections.Generic;
using UnityEngine;

//PersistenceKeeper is exclusively used to toss onto something to ensure that it does not get destroyed upon loading in
public class PersistenceKeeper : MonoBehaviour
{
    private static List<string> existingObjects = new();
    // Start is called before the first frame update
    void Awake()
    {
        //Do not destroy the gameobject
        if (existingObjects.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log(gameObject.name);
            DontDestroyOnLoad(gameObject);
            existingObjects.Add(gameObject.name);
        }
    }
}

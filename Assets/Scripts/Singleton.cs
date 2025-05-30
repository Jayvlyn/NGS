using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    [SerializeField] protected bool overrideOthers = false;
    [SerializeField] protected bool destroyOnLoad = false;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if(!destroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else if(overrideOthers)
        {
            Destroy(instance.gameObject);
            instance = this as T;
            if (!destroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

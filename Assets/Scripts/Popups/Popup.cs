using GameEvents;
using UnityEngine;

public abstract class Popup<T> : MonoBehaviour
{
    public BaseGameEvent<T> Event;
    public float Lifetime = -1; 
    [SerializeField] protected T defaultResponse;

    protected virtual void FixedUpdate()
    {
       if(Lifetime != -1)
       {
            Lifetime -= Time.fixedDeltaTime;
            if(Lifetime < 0 )
            {
                ClosePopup(defaultResponse);
            }
       }
    }

    public virtual void ClosePopup(T result)
    {
        if(Event != null)
        {
            Event.Raise(result);
        }
        Destroy(gameObject);
    }
}

using GameEvents;
using UnityEngine;

public abstract class Popup<T> : MonoBehaviour
{
    public BaseGameEvent<T> Event;
    [SerializeField] protected float lifetime = -1; 
    [SerializeField] protected T defaultResponse;

    protected virtual void FixedUpdate()
    {
       if(lifetime != -1)
       {
            lifetime -= Time.fixedDeltaTime;
            if(lifetime < 0 )
            {
                ClosePopup(defaultResponse);
            }
       }
    }

    public virtual void ClosePopup(T result)
    {
        Event.Raise(result);
        Destroy(gameObject);
    }
}

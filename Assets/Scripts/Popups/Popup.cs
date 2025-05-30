using GameEvents;
using UnityEngine;

public abstract class Popup<T> : MonoBehaviour
{
    public BaseGameEvent<T> Event;
    public float Lifetime = -1; 
    [SerializeField] protected T defaultResponse;
    public PopupAppearanceData closeBehavior;
    protected bool active = true;
    public GameObject killObject;

    private void Awake()
    {
        if (killObject == null)
        {
            killObject = gameObject;
        }
    }

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
        if (active)
        {
            if (Event != null)
            {
                Event.Raise(result);
            }
            active = false;
            if(gameObject.TryGetComponent(out PearanceHandler handler))
            {
                Destroy(handler);
            }
            switch(closeBehavior.AppearanceType)
            {
                case AppearanceType.FadeIn:
                    Fader fader = gameObject.AddComponent<Fader>();
                    fader.time = closeBehavior.Time;
                    fader.killObject = killObject;
                    break;
                case AppearanceType.ZoomIn:
                    Zoomer zoomer = gameObject.AddComponent<Zoomer>();
                    zoomer.time = closeBehavior.Time;
                    zoomer.killObject = killObject;
                    break;
                case AppearanceType.FromBottom:
                case AppearanceType.FromTop:
                    closeBehavior.Offset *= Screen.height;
                    Flyer component = gameObject.AddComponent<Flyer>();
                    component.fromDirection = (Direction)(int)(closeBehavior.AppearanceType - 3);
                    component.offset = closeBehavior.Offset;
                    component.time = closeBehavior.Time;
                    component.killObject = killObject;
                    break;
                case AppearanceType.FromRight:
                case AppearanceType.FromLeft:
                    closeBehavior.Offset *= Screen.width;
                    component = gameObject.AddComponent<Flyer>();
                    component.fromDirection = (Direction)(int)(closeBehavior.AppearanceType - 3);
                    component.offset = closeBehavior.Offset;
                    component.time = closeBehavior.Time;
                    component.killObject = killObject;
                    break;
                default:
                    Destroy(gameObject);
                    return;
            }
        }
    }
}

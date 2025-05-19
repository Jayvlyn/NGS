using GameEvents;
using UnityEngine;

public class VoidPopup : Popup<Void>
{
    public override void ClosePopup(Void result)
    {
        if (active)
        {
            if (Event != null)
            {
                ((VoidEvent)Event).Raise();
            }
            active = false;
            if (gameObject.TryGetComponent(out PearanceHandler handler))
            {
                Destroy(handler);
            }
            switch (closeBehavior.AppearanceType)
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

    public void ClosePopup()
    {
        ClosePopup(new Void());
    }
}

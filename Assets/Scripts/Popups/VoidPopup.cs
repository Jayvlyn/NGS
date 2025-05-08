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
            switch (closeBehavior.AppearanceType)
            {
                case AppearanceType.FadeIn:
                    Fader fader = gameObject.AddComponent<Fader>();
                    fader.time = closeBehavior.Time;
                    fader.closing = true;
                    break;
                case AppearanceType.ZoomIn:
                    Zoomer zoomer = gameObject.AddComponent<Zoomer>();
                    zoomer.time = closeBehavior.Time;
                    zoomer.closing = true;
                    break;
                case AppearanceType.FromBottom:
                case AppearanceType.FromTop:
                    closeBehavior.Offset *= Screen.height;
                    Flyer component = gameObject.AddComponent<Flyer>();
                    component.fromDirection = (Direction)(int)(closeBehavior.AppearanceType - 3);
                    component.offset = closeBehavior.Offset;
                    component.time = closeBehavior.Time;
                    component.closing = true;
                    break;
                case AppearanceType.FromRight:
                case AppearanceType.FromLeft:
                    closeBehavior.Offset *= Screen.width;
                    component = gameObject.AddComponent<Flyer>();
                    component.fromDirection = (Direction)(int)(closeBehavior.AppearanceType - 3);
                    component.offset = closeBehavior.Offset;
                    component.time = closeBehavior.Time;
                    component.closing = true;
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

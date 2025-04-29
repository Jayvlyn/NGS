using GameEvents;
using UnityEngine;

public class VoidPopup : Popup<Void>
{
    public override void ClosePopup(Void result)
    {
        ((VoidEvent)Event).Raise();
    }
}

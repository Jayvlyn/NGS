using GameEvents;
using UnityEngine;

public class VoidPopup : Popup<Void>
{
    public override void ClosePopup(Void result)
    {
        if (Event != null)
        {
            ((VoidEvent)Event).Raise();
        }
        Destroy(gameObject);
    }

    public void ClosePopup()
    {
        ClosePopup(new Void());
    }
}

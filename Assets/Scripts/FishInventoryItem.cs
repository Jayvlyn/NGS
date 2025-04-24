using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

public class FishInventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FishEvent fishEventOnHover;
    public VoidEvent fishEventStopHover;
    public Fish fish;
    private bool isHovering = false;    
    private void OnDisable()
    {
        if (isHovering)
        { 
            fishEventStopHover.Raise();
            isHovering = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        fishEventOnHover.Raise(fish);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        fishEventStopHover.Raise();
        isHovering = false;
    }

}

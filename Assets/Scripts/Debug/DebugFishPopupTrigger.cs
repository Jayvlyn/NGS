using UnityEngine;

public class DebugFishPopupTrigger : InteractableObject
{
    [SerializeField] Fish fish;
    [SerializeField, Tooltip("0: Generic Fish Caught\n1:New Largest Fish Caught\n2:New Fish Type Caught")] private int popupType = 0;
    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj?.Id == Id)
        {
           switch(popupType)
            {
                case 1:
                    PopupManager.Instance.CreateNewLargestFishPopup(fish); 
                    break;
                case 2:
                    PopupManager.Instance.CreateNewFishTypePopup(fish);
                    break;
                default:
                    PopupManager.Instance.CreateGenericFishPopup(fish); 
                    break;
            }
        }
    }
}

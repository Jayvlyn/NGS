using UnityEngine;

public class Landmark : InteractableObject
{
    [SerializeField] protected Transform dialoguePopupLocation;
    [SerializeField] protected string landmarkDescription;
    [SerializeField] protected string landmarkName;
    [SerializeField] protected bool screenPopup;
    [SerializeField] protected float lifetime = 30;

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            if(screenPopup)
            {
                PopupManager.Instance.CreateScreenStatementPopup(landmarkDescription, lifetime, landmarkName);
            }
            else
            {

                PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupLocation, landmarkDescription, lifetime, landmarkName);
            }
        }
    }
}

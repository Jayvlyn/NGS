using UnityEngine;

public class Landmark : InteractableObject
{
    [SerializeField] protected Transform dialoguePopupLocation;
    [SerializeField] protected string[] landmarkDescriptions;
    [SerializeField] protected bool loops = false;
    [SerializeField] protected string landmarkName;
    [SerializeField] protected bool screenPopup;
    [SerializeField] protected float lifetime = 30;
    [SerializeField] protected PopupAppearanceData appearanceData;
    protected int current = 0;

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            if(screenPopup)
            {
                PopupManager.Instance.CreateScreenStatementPopup(landmarkDescriptions[current], lifetime, landmarkName, appearanceData);
            }
            else
            {

                PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupLocation, landmarkDescriptions[current], lifetime, landmarkName, appearanceData);
            }
            current++;
            if(current == landmarkDescriptions.Length)
            {
                current = loops ? 0 : current - 1;
            }
        }
    }
}

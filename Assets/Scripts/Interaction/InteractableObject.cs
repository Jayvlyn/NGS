using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    public int Id {  get; private set; }
    private static int count = 0;
    public InteractionType interactionType;
    [SerializeField] protected InteractionEvent enterInteractionRangeEvent;
    [SerializeField] protected InteractionEvent exitInteractionRangeEvent;
    [SerializeField] protected InteractionEvent interactEvent;
    [SerializeField] protected Transform popupLocation;
    protected Dictionary<int, GameObject> currentPopups = new();
    protected virtual void Start()
    {
        Id = count;
        count++;
        if (interactEvent != null)
        {
            interactEvent.Subscribe(Interact);
        }
    }

    protected virtual void Interact(InteractionPair pair)
    {
        if (pair.obj.Id == Id)
        {
            Debug.Log($"{pair.actor.Id} interacted with {pair.obj.Id}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Interactor actor))
        {
            enterInteractionRangeEvent.Trigger(new InteractionPair(this, actor));
            if(!currentPopups.ContainsKey(actor.Id))
            {
                currentPopups.Add(actor.Id, PopupManager.Instance.CreateWorldInteractionPopup(popupLocation));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Interactor actor))
        {
            exitInteractionRangeEvent.Trigger(new InteractionPair(this, actor));
            if (currentPopups.ContainsKey(actor.Id))
            {
                Destroy(currentPopups[actor.Id]);
                currentPopups.Remove(actor.Id);
            }
        }
    }
}

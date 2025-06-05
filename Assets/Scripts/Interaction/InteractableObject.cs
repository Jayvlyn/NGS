using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    public int Id { get; protected set; } = -1;
    private static int count = 0;
    public InteractionType interactionType;
    public InteractionEvent enterInteractionRangeEvent;
    public InteractionEvent exitInteractionRangeEvent;
    public InteractionEvent interactEvent;
    public Transform popupLocation;
    //protected Dictionary<int, GameObject> currentPopups = new();
    protected virtual void Start()
    {
        if (Id == -1)
        {
            Id = count;
            count++;
        }
        if (interactEvent != null)
        {
            interactEvent.Subscribe(Interact);
        }
    }
    public void CreateId(int newId)
    {
        if(Id == -1)
        {
            Id = newId;
        }
    }

    protected virtual void Interact(InteractionPair pair)
    {
        if (pair.obj.Id == Id)
        {
            Debug.Log($"{pair.actor.Id} interacted with {pair.obj.Id}");
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Interactor actor))
        {
            enterInteractionRangeEvent.Trigger(new InteractionPair(this, actor));
            //if(!currentPopups.ContainsKey(actor.Id))
            //{
            //    currentPopups.Add(actor.Id, PopupManager.Instance.CreateWorldInteractionPopup(popupLocation));
            //}
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Interactor actor))
        {
            InteractionPair pair = new(this, actor);
            exitInteractionRangeEvent.Trigger(pair);
            //if (currentPopups.ContainsKey(actor.Id))
            //{
            //    Destroy(currentPopups[actor.Id]);
            //    currentPopups.Remove(actor.Id);
            //}
        }
    }
}

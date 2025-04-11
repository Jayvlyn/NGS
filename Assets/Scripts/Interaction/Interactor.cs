using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Interactor : MonoBehaviour
{
    private static int current;
    public int Id { get; private set; }
    protected Stack<InteractableObject> objectStack = new();
    //[SerializeField] protected KeyCode interactionKey = KeyCode.Space;
    [SerializeField] protected InteractionEvent enterInteractionRangeEvent;
    [SerializeField] protected InteractionEvent exitInteractionRangeEvent;
    [SerializeField] protected InteractionEvent interactEvent;
    
    public void OnEnterInteractableRange(InteractionPair pair)
    {
        if (pair.actor.Id == Id)
        {
            //Debug.Log($"{pair.actor.Id} entered range of {pair.obj.Id}");
            objectStack.Push(pair.obj);
        }
    }

    public void OnExitInteractableRange(InteractionPair pair)
    {
        if (pair.actor.Id == Id)
        {
            Stack<InteractableObject> holder = new();
            while (objectStack.Count > 0 && objectStack.Peek().Id != pair.obj.Id)
            {
                holder.Push(objectStack.Pop());
            }
            if (objectStack.Count > 0)
            {
                objectStack.Pop();
            }
            while (holder.Count > 0)
            {
                objectStack.Push(holder.Pop());
            }
            //Debug.Log($"{pair.actor.Id} exited range of {pair.obj.Id}");
        }
    }

    //public virtual void Update()
    //{
    //    if (Input.GetKeyDown(interactionKey))
    //    {
    //        TryInteract();
    //    }
    //}

    protected void TryInteract()
    {
		if (objectStack.Count > 0)
		{
			interactEvent.Trigger(new InteractionPair(objectStack.Peek(), this));
		}
	}

    public virtual void Start()
    {
        Id = current;
        current++;
        enterInteractionRangeEvent.Subscribe(OnEnterInteractableRange);
        exitInteractionRangeEvent.Subscribe(OnExitInteractableRange);
    }
}

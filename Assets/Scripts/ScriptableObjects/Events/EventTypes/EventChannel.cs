using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Event", menuName = "Scriptable Objects/Event")]
public abstract class EventChannel<T> : ScriptableObject
{
    public UnityAction<T> actions;

    public virtual void Trigger(T value)
    {
        actions.Invoke(value);
        int caow = 9;
        Debug.Log(caow);
    }

    public void Subscribe(UnityAction<T> action)
    {
        if(action != null)
        {
            actions += action;
        }
    }

    public void Unsubscribe(UnityAction<T> action)
    {
        if(action != null)
        {
            actions -= action;
        }
    }

}

[CreateAssetMenu(menuName = "Events/EmptyEvent")]
public class EmptyEvent : EventChannel<None>
{
    private readonly Dictionary<UnityAction, UnityAction<None>> wrappedActions = new();
    public override void Trigger(None doNotUse = new None())
    {
        base.Trigger(doNotUse);
    }

    public void Subscribe(UnityAction action)
    {
        if(action != null)
        {
            void wrapper(None _) => action();
            wrappedActions[action] = wrapper;
            Subscribe(wrapper);
        }
    }

    public void Unsubscribe(UnityAction action)
    {
        if (action != null && wrappedActions.TryGetValue(action, out var wrapper))
        {
            base.Unsubscribe(wrapper);
            wrappedActions.Remove(action);
        }
    }
}
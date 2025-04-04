using System.Collections;
using UnityEngine;

public struct InteractionPair
{
    public InteractableObject obj;
    public Interactor actor;

    public InteractionPair(InteractableObject obj, Interactor actor)
    {
        this.obj = obj;
        this.actor = actor;
    }
}

using UnityEngine;

public class WardrobeInteraction : InteractableObject
{
    private WardrobeManager wardrobeManager;

    protected override void Start()
    {
        base.Start();
        wardrobeManager = FindFirstObjectByType<WardrobeManager>(FindObjectsInactive.Include);
    }

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            wardrobeManager.OpenWardrobe();
        }
    }
}

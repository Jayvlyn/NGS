using UnityEngine;

public class WardrobeInteraction : InteractableObject
{
    [SerializeField] public WardrobeManager wardrobeManager;

    protected override void Start()
    {
        base.Start();
        wardrobeManager = FindFirstObjectByType<WardrobeManager>();
    }

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            wardrobeManager.OpenWardrobe();
        }
    }
}

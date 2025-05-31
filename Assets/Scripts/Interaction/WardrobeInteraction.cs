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
        wardrobeManager = FindFirstObjectByType<WardrobeManager>(FindObjectsInactive.Include);
        if (pair.obj.Id == Id && !GameUI.Instance.pause.activeSelf)
        {
            wardrobeManager.OpenWardrobe();
        }
    }
}

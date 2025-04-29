using UnityEngine;

public class DebugFishAdder : MonoBehaviour
{
    [SerializeField] private Fish[] fishToAdd;
    [SerializeField] private int[] lengths;
    public void Start()
    {
        for(int i = 0; i < fishToAdd.Length; i++)
        {
            Fish f = Instantiate(fishToAdd[i]);
            f.length = lengths[i];
            Inventory.Instance.AddFish(f);
        }
    }
}

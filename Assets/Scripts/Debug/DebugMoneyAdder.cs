using UnityEngine;

public class DebugMoneyAdder : MonoBehaviour
{
    [SerializeField] private float moneyToAdd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Inventory.Instance.AddMoney(moneyToAdd);
    }
}

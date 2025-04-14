using GameEvents;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WaterStateController : MonoBehaviour
{
    [SerializeField] private BoolEvent waterStateEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        waterStateEvent.Raise(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        waterStateEvent.Raise(false);
    }
}

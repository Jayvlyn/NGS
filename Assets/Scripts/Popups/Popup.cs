using GameEvents;
using UnityEngine;

public abstract class Popup<T> : MonoBehaviour
{
    public BaseGameEvent<T> Event;
}

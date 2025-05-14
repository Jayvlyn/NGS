using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public Transform toTrack;
    void Update()
    {
        transform.position = toTrack.position;
    }
}

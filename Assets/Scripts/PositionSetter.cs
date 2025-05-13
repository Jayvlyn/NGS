using UnityEngine;

public class PositionSetter : MonoBehaviour
{
    public Transform posToSet;
    public KeyCode debugKey = KeyCode.P;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            posToSet.position = transform.position;
        }
    }
#endif
}

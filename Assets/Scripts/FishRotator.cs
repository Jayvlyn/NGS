using UnityEngine;

public class FishRotator : MonoBehaviour
{
    [HideInInspector] public Quaternion BaseRotation;
    [SerializeField] private float min;
    [SerializeField] private float max;
    [SerializeField] private bool loop;
    [SerializeField] private float speed;
    [SerializeField] private bool increasing = true;
    [SerializeField] private Rigidbody2D rb;
    private float current = 0;

    private void Update()
    {
        current += speed * (increasing ? Time.deltaTime : -Time.deltaTime);
        if(current > max)
        {
            if(loop)
            {
                current -= max;
                current += min;
            }
            else
            {
                current = max - (current - max);
                increasing = false;
            }
        }
        else if(current < min)
        {
            if(loop)
            {
                current -= min;
                current += max;
            }
            else
            {
                current = min - (current - min);
                increasing = true;
            }
        }
        rb.MoveRotation(BaseRotation * Quaternion.Euler(0, 0, current));
    }
}

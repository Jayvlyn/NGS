using System.Drawing.Text;
using UnityEngine;

public class DampedFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private int damping = 2;
    private int delay;
    private void Start()
    {
        delay = damping;
    }
    void Update()
    {
        transform.rotation = target.rotation;
        if (damping > 0)
        {
            if(delay == 0)
            {
                transform.position += (target.position - transform.position) / damping;
            }
            else
            {
                delay--;
            }
        }
        else
        {
            transform.position = target.position;
        }
    }
}

using UnityEngine;

public class RodLine : MonoBehaviour
{
    [SerializeField] GameObject startPosition;
    [SerializeField] GameObject endPosition;

    [SerializeField] LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, startPosition.transform.position);
        lineRenderer.SetPosition(1, endPosition.transform.position);
    }

    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, startPosition.transform.position);
        lineRenderer.SetPosition(1, endPosition.transform.position);
    }
}

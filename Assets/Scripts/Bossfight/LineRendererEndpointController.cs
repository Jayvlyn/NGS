using UnityEngine;
[RequireComponent (typeof(LineRenderer))]
public class LineRendererEndpointController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;
    void Start()
    {
        if(lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, from.position);
        lineRenderer.SetPosition(1, to.position);
    }
}

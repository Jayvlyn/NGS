using UnityEngine;

public class ParticleHoming : MonoBehaviour
{
    public RectTransform uiTarget; // Drag your money icon UI here
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    public float speed = 10f;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        //uiTarget = transform.Find("money").GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (ps == null || uiTarget == null) return;

        int count = ps.GetParticles(particles);

        Vector3 worldTarget = GetUIWorldPosition(uiTarget);

        for (int i = 0; i < count; i++)
        {
            Debug.DrawLine(particles[i].position, worldTarget, Color.green, 0.1f);
            Vector3 dir = (worldTarget - particles[i].position).normalized;
            particles[i].velocity = dir * speed;
        }

        ps.SetParticles(particles, count);
    }

    //Vector3 GetUIWorldPosition(RectTransform uiElement)
    //{
    //    Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, uiElement.position);
    //    screenPos.z = 10f; // Make sure it’s in front of the camera
    //    return Camera.main.ScreenToWorldPoint(screenPos);
    //}

    Vector3 GetUIWorldPosition(RectTransform uiElement)
    {
        // This assumes the canvas is in Screen Space - Overlay
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiElement.localPosition);
        screenPos.z = 10f; // Set depth in front of the camera
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}

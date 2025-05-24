using UnityEngine;

public class ParticleHoming : MonoBehaviour
{
    public RectTransform uiTarget;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    public float speed = 10f;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
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

    Vector3 GetUIWorldPosition(RectTransform uiElement)
    {
        // This assumes the canvas is in Screen Space - Overlay
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiElement.position);
        screenPos.y *= 1.7f;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}

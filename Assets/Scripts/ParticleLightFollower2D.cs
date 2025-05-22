using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class ParticleLightFollower2D : MonoBehaviour
{
	public ParticleSystem ps;
	public GameObject lightPrefab;
	public int maxLights = 50;

	private ParticleSystem.Particle[] particles;
	private List<Light2D> activeLights = new List<Light2D>();

	void Awake()
	{
		if (ps == null)
			ps = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		int maxParticles = ps.main.maxParticles;

		if (particles == null || particles.Length < maxParticles)
			particles = new ParticleSystem.Particle[maxParticles];

		int count = ps.GetParticles(particles);

		// Ensure we have enough lights
		while (activeLights.Count < count && activeLights.Count < maxLights)
		{
			var newLightObj = Instantiate(lightPrefab, transform);
			var newLight = newLightObj.GetComponent<Light2D>();
			newLightObj.transform.localScale = Vector3.one;
			activeLights.Add(newLight);
		}

		for (int i = 0; i < activeLights.Count; i++)
		{
			if (i < count)
			{
				activeLights[i].gameObject.SetActive(true);

				Vector3 particlePos = particles[i].position;
				// Map particle (x, y) to (x, 0, y)
				activeLights[i].transform.localPosition = new Vector3(particlePos.x, -0.01f, particlePos.y);

				// Optional: match light intensity to particle alpha
				activeLights[i].intensity = particles[i].GetCurrentColor(ps).a;
			}
			else
			{
				activeLights[i].gameObject.SetActive(false);
			}
		}
	}
}

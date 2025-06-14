using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleFX : MonoBehaviour
{
    [Header("Particle Settings")]
    public GameObject particlePrefab;
    public RectTransform targetUI;
    public float speed = 1f;
    public float lifetime = 5f;

    private List<GameObject> pool = new List<GameObject>();

    public void SpawnParticles(int count, Vector3 screenPosition, bool quest = false)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = GetParticleFromPool();
            RectTransform rt = go.GetComponent<RectTransform>();

            // Add some screen-space randomness
            Vector2 randomizedScreenPos = new Vector2(
                Random.Range(screenPosition.x - 50, screenPosition.x + 50),
                Random.Range(screenPosition.y - 50, screenPosition.y + 50)
            );

            // Convert to local position in the canvas
            Vector2 localPoint;
            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                randomizedScreenPos,
                null, // assumes screen space overlay
                out localPoint
            );

            rt.anchoredPosition = localPoint;

            go.SetActive(true);
            StartCoroutine(MoveToTarget(rt, quest));
        }
    }

    IEnumerator MoveToTarget(RectTransform particle, bool quest)
    {
        float timer = 0f;

        while (timer < lifetime && Vector3.Distance(particle.position, targetUI.position) > 10f)
        {
            Vector3 dir = (targetUI.position - particle.position).normalized;
            if (quest)
            {
                dir.y *= 2f;
                timer += Time.deltaTime * 0.8f;
            }
            particle.position += dir * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        particle.gameObject.SetActive(false);
    }

    private GameObject GetParticleFromPool()
    {
        pool.RemoveAll(p => p == null);

        foreach (var p in pool)
        {
            if (!p.activeInHierarchy) return p;
        }

        GameObject newParticle = Instantiate(particlePrefab, transform.Find("CarrotPool").transform);
        newParticle.SetActive(false);
        pool.Add(newParticle);
        return newParticle;
    }
}

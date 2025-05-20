using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffectx = 0.1f;
    [SerializeField] private float parallaxEffecty = 0.1f;

    [SerializeField] private bool loopY;
    [SerializeField] private bool dontGoBelowZero = true;

    private float startposx, startposy;
    private float lengthx, lengthy;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogWarning("Camera reference is missing!", this);
            enabled = false;
            return;
        }

        startposx = transform.position.x;
        startposy = transform.position.y;
        lengthx = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthy = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void LateUpdate()
    {
        float distancex = cam.transform.position.x * parallaxEffectx;
        float distancey = cam.transform.position.y * parallaxEffecty;

        float newY = startposy + distancey;
        if (dontGoBelowZero && newY < 0)
            newY = 0;

        transform.position = new Vector3(startposx + distancex, newY, transform.position.z);

        float camDisplacementX = cam.transform.position.x * (1 - parallaxEffectx);
        if (camDisplacementX > startposx + lengthx) startposx += lengthx;
        else if (camDisplacementX < startposx - lengthx) startposx -= lengthx;

        if (loopY)
        {
            float camDisplacementY = cam.transform.position.y * (1 - parallaxEffecty);
            if (camDisplacementY > startposy + lengthy) startposy += lengthy;
            else if (camDisplacementY < startposy - lengthy) startposy -= lengthy;
        }
    }
}

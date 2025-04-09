using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float startposx;
    [SerializeField] float startposy;
    [SerializeField] float lengthx;
    [SerializeField] float lengthy;
    [SerializeField] GameObject cam;
    [SerializeField] float parallaxEffectx; // The effect of the parallax, 0.1f is a good value 0 will move with the camera 1 will not 
    [SerializeField] float parallaxEffecty; // The effect of the parallax, 0.1f is a good value 0 will move with the camera 1 will not 

    void Start()
    {
        startposx = transform.position.x;
        startposy = transform.position.y;
        lengthx = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthy = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void LateUpdate() // Late update removes jitter
    {
        float distancex = (cam.transform.position.x * parallaxEffectx);
        float distancey = (cam.transform.position.y * parallaxEffecty);
        float movementx = (cam.transform.position.x * (1 - parallaxEffectx));
        float movementy = (cam.transform.position.y * (1 - parallaxEffecty));

        float newY = startposy + distancey;
        if (newY < 0 || parallaxEffecty == -1) newY = 0;
        
        transform.position = new Vector3(startposx + distancex, newY, transform.position.z);

        if (movementx > startposx + lengthx) startposx += lengthx;
        else if (movementx < startposx - lengthx) startposx -= lengthx;

        if (movementy > startposy + lengthy) startposy += lengthy;
        else if (movementy < startposy - lengthy) startposy -= lengthy;
        
    }
}

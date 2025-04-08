using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float startposx;
    [SerializeField] float length;
    [SerializeField] GameObject cam;
    [SerializeField] float parallaxEffect; // The effect of the parallax, 0.1f is a good value 0 will move with the camera 1 will not 

    void Start()
    {
        startposx = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (cam.transform.position.x * parallaxEffect);
        float movement = (cam.transform.position.x * (1 - parallaxEffect));
        transform.position = new Vector3(startposx + distance, transform.position.y, transform.position.z);

        if (movement > startposx + length) startposx += length;
        else if (movement < startposx - length) startposx -= length;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CastSystem : MonoBehaviour
{
    [SerializeField] GameObject CastScreen;
    [SerializeField] Scrollbar castBar;

    private float increment = 0.01f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CastScreen.SetActive(false);
        }
        if (Input.anyKeyDown)
        {
            print("You got a " + castBar.value.ToString());
        }
    }

    private void FixedUpdate()
    {
        if (castBar.value >= 1) increment = -0.01f;
        else if (castBar.value <= 0) increment = 0.01f;

        castBar.value += increment;
    }
}

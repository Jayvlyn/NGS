using GameEvents;
using System.Xml.Serialization;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    private static int runningCount = 0;
    [SerializeField] private GameObject imageObject;
    [SerializeField] private Transform platformPlacement;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject currentPlatform;
    [SerializeField] private float shakeTime;
    private float currentTime = -1;
    [SerializeField] private float shakeAggression;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float fallTime;
    [SerializeField] private float regenCooldown;
    [SerializeField] private PopupAppearanceData appearance;
    [SerializeField] private BaseGameEvent<int> fallEvent;
    private int currentInsideCount = 0;
    public int Id { get; private set; }
    private void Start()
    {
        Id = runningCount;
        runningCount++;
    }
    private void Update()
    {
        if(imageObject == null && currentPlatform != null)
        {
            imageObject = currentPlatform.GetComponentInChildren<SpriteRenderer>().gameObject;
        }
        if(currentTime != -1)
        {
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                if(imageObject != null && imageObject.GetComponent<Shaker>() != null)
                {
                    Faller faller = currentPlatform.AddComponent<Faller>();
                    faller.velocity = new Vector3(0, -fallSpeed);
                    faller.lifetime = fallTime;
                    currentTime = fallTime;
                    fallEvent.Raise(Id);
                }
                else if(currentPlatform != null)
                {
                    currentTime = regenCooldown;
                }
                else
                {
                    currentTime = -1;
                    currentPlatform = Instantiate(platformPrefab, platformPlacement);
                    switch(appearance.AppearanceType)
                    {
                        case AppearanceType.FadeIn:
                            {
                                currentPlatform.AddComponent<Fader>().time = appearance.Time;
                                break;
                            }
                        case AppearanceType.ZoomIn:
                            {
                                currentPlatform.AddComponent<Zoomer>().time = appearance.Time;
                                break;
                            }
                        case AppearanceType.FromBottom:
                        case AppearanceType.FromTop:
                        case AppearanceType.FromRight:
                        case AppearanceType.FromLeft:
                            Flyer flyer = currentPlatform.AddComponent<Flyer>();
                            flyer.fromDirection = (Direction)(int)(appearance.AppearanceType - 3);
                            flyer.offset = appearance.Offset;
                            flyer.time = appearance.Time;
                            break;
                    }
                    if(currentInsideCount != 0)
                    {
                        imageObject = currentPlatform.GetComponentInChildren<SpriteRenderer>().gameObject;
                        AddShaker();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentInsideCount++;
        if(imageObject != null && imageObject.GetComponentInChildren<Shaker>() == null && currentPlatform.GetComponentInChildren<Faller>() == null)
        {
            AddShaker();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInsideCount--;
        if (imageObject != null)
        {
            Shaker shaker = imageObject.GetComponent<Shaker>();
            if (currentInsideCount == 0 && shaker != null)
            {
                shaker.lifetime = -1;
                currentTime = -1;
            }
        }
    }

    private void AddShaker()
    {
        Shaker shaker = imageObject.AddComponent<Shaker>();
        shaker.lifetime = shakeTime;
        shaker.shakeAggression = shakeAggression;
        currentTime = shakeTime;
    }
}

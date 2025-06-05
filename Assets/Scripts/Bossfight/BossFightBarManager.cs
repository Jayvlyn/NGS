using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(MinigameProgressBar))]
[RequireComponent(typeof(Slider))]
public class BossFightBarManager : MonoBehaviour
{
    [SerializeField] private BossFishController boss;
    private Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = boss.GetRemainingDistance();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = boss.GetRemainingDistance();
    }
}

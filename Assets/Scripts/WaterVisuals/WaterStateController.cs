using GameEvents;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WaterStateController : MonoBehaviour
{
    TMP_Text debugTxt;
	private void Start()
	{
	    debugTxt = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
	}

	[SerializeField] private BoolEvent waterStateEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        debugTxt.text = "Entered";
        waterStateEvent.Raise(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        debugTxt.text = "Exited";
        waterStateEvent.Raise(false);
    }
}

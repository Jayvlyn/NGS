using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class BossfightLevelSection : MonoBehaviour
{
    [SerializeField] private int endPosition = 0;
    [SerializeField] private SplineContainer spline;
    public int EndPosition { get { return endPosition; } }
    public SplineContainer Spline { get { return spline; } }
    public BossfightLevelSection Next { get; set; }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("Player Entered Win Area");
		//Inventory.Instance.AddFish(BossFishController.bossFish);
		BossFishController.caughtBoss = true;
		SceneManager.LoadScene("GameScene");
	}
}

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
}

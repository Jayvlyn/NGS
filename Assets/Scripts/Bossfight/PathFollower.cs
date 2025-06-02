using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] Vector3 rotatedBy = Vector3.zero;
    [SerializeField] BossfightLevelSection guide;
    public GameSettings settings;

    [SerializeField, Range(0, 1)]float distance = 0; //distance along spline, 0-1]
    public float Length { get { return splineContainer.CalculateLength(); } }
	public (Vector3, Quaternion) GetNewTransform(float distanceTravelled)
    {
        (Vector3, Quaternion) result = (transform.position, Quaternion.identity);
        if (guide != null)
        {
            if (distance + distanceTravelled / Length < 1)
            {
                distance += distanceTravelled / Length;
            }
            else
            {
                guide = guide.Next;
                if(guide == null)
                {
                    distance = 1;
					SceneLoader.LoadScene(settings.position.currentLocation);
				}
                else
                {
                    splineContainer = guide.Spline;
                    distance += (distanceTravelled / Length) - 1;
                }
            }
            Vector3 position = splineContainer.EvaluatePosition(distance);
            //Vector3 up = splineContainer.EvaluateUpVector(distance);
            //Debug.Log(up);
            //Vector3 forward = splineContainer.EvaluateTangent(distance);
            Vector3 relative = (position - transform.position).normalized;
			result = (position, Quaternion.Euler(0, 0, 
                Mathf.Rad2Deg * Mathf.Atan2(relative.y, relative.x))
                * Quaternion.Euler(rotatedBy));
            //result.Item2 *= Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
		}
        return result;
    }

    private void Start()
    {
        if(splineContainer == null && guide != null)
        {
            splineContainer = guide.Spline;
        }
        else if(splineContainer != null && guide == null)
        {
            guide = splineContainer.gameObject.GetComponentInParent<BossfightLevelSection>();
        }
    }
}

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
                }
                else
                {
                    splineContainer = guide.Spline;
                    distance += (distanceTravelled / Length) - 1;
                }
            }
            Vector3 position = splineContainer.EvaluatePosition(distance);
            Vector3 up = splineContainer.EvaluateUpVector(distance);
            Debug.Log(up);
            result = (position, Quaternion.LookRotation(Vector3.forward, up) * Quaternion.Euler(rotatedBy));
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

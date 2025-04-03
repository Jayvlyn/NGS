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
    [SerializeField] GameObject guide;

    [SerializeField, Range(0, 1)]float distance = 0; //distance along spline, 0-1]
    public float length { get { return splineContainer.CalculateLength(); } }
	public (Vector3, Quaternion) GetNewTransform(float distanceTravelled)
    {
        distance += distanceTravelled / length;
        Vector3 position = splineContainer.EvaluatePosition(distance);
        Vector3 up = splineContainer.EvaluateUpVector(distance);
        Vector3 forward = splineContainer.EvaluateTangent(distance);
        return (position, Quaternion.LookRotation(forward, up) * Quaternion.Euler(rotatedBy));
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    public Direction fromDirection;
    public float offset;
    public float time;
    private float currentTime;
    private readonly List<(Transform, float)> affectedTransforms = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform t in transform)
        {
            affectedTransforms.Add((t, (fromDirection == Direction.Up || fromDirection == Direction.Down) ? t.localPosition.y : t.localPosition.x));
        }

        if(affectedTransforms.Count > 0)
        {
            foreach ((Transform, float) trans in affectedTransforms)
            {
                switch (fromDirection)
                {
                    case Direction.Up:
                    case Direction.Down:
                        trans.Item1.localPosition = new Vector3(trans.Item1.localPosition.x, trans.Item2 + offset * (fromDirection == Direction.Up ? -1 : 1), trans.Item1.localPosition.z);
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        trans.Item1.localPosition = new Vector3(trans.Item2 + offset * (fromDirection == Direction.Left ? 1 : -1), trans.Item1.localPosition.y, transform.localPosition.z);
                        break;
                }
            }
            currentTime = 0;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > time)
        {
            foreach ((Transform, float) trans in affectedTransforms)
            {
                switch (fromDirection)
                {
                    case Direction.Up:
                    case Direction.Down:
                        trans.Item1.localPosition = new Vector3(trans.Item1.localPosition.x, trans.Item2, trans.Item1.localPosition.z);
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        trans.Item1.localPosition = new Vector3(trans.Item2, trans.Item1.localPosition.y, trans.Item1.localPosition.z);
                        break;
                }
            }
            Destroy(this);
        }
        else
        {
            foreach ((Transform, float) trans in affectedTransforms)
            {
                float completion = offset * (1 - currentTime / time);
                switch (fromDirection)
                {
                    case Direction.Up:
                    case Direction.Down:
                        trans.Item1.localPosition = new Vector3(trans.Item1.localPosition.x, trans.Item2 + completion * (fromDirection == Direction.Up ? -1 : 1), trans.Item1.localPosition.z);
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        trans.Item1.localPosition = new Vector3(trans.Item2 + completion * (fromDirection == Direction.Left ? 1 : -1), trans.Item1.localPosition.y, trans.Item1.localPosition.z);
                        break;
                }
            }
        }
    }
}

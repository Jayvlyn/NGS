using UnityEngine;

public class Flyer : MonoBehaviour
{
    public Direction fromDirection;
    public float offset;
    private float initialPosition;
    public float time;
    private float currentTime;
    private Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        if(canvas != null)
        {
            switch(fromDirection)
            {
                case Direction.Up:
                case Direction.Down:
                    initialPosition = canvas.transform.localPosition.y;
                    canvas.transform.localPosition = new Vector3(canvas.transform.localPosition.x, initialPosition + offset, canvas.transform.localPosition.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    initialPosition = canvas.transform.localPosition.x;
                    canvas.transform.localPosition = new Vector3(initialPosition + offset, canvas.transform.localPosition.y, transform.localPosition.z);
                    break;
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
            switch (fromDirection)
            {
                case Direction.Up:
                case Direction.Down:
                    canvas.transform.localPosition = new Vector3(canvas.transform.localPosition.x, initialPosition, canvas.transform.localPosition.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    canvas.transform.localPosition = new Vector3(initialPosition, canvas.transform.localPosition.y, canvas.transform.localPosition.z);
                    break;
            }
            Destroy(this);
        }
        else
        {
            float completion = offset * (1 - currentTime / time);
            switch(fromDirection)
            {
                case Direction.Up:
                case Direction.Down:
                    canvas.transform.localPosition = new Vector3(canvas.transform.localPosition.x, initialPosition + completion, canvas.transform.localPosition.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    canvas.transform.localPosition = new Vector3(initialPosition + completion, canvas.transform.localPosition.y, canvas.transform.localPosition.z);
                    break;
            }
        }
    }
}

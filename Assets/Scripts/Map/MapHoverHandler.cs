using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapHoverHandler : MonoBehaviour
{
    public List<Fish> fishToDisplay;

    private static bool canOpenNew = true;
    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canOpenNew)
        {
            Vector3Int pos = MapDisplay.Instance.WorldToCell(collision.transform.position);
            if(MapManager.Instance.GetVisibleTiles().Contains((pos.x, pos.y)))
            {
                AddPopupToScreen();
            }
        }
    }

    private void AddPopupToScreen()
    {
        isOpen = true;
        canOpenNew = false;

        //TODO: Connect UI element logic
    }

    private void RemovePopupFromScreen()
    {
        isOpen = false;
        canOpenNew = true;

        //TODO: Connect UI element logic
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(canOpenNew)
        {
            Vector3Int pos = MapDisplay.Instance.WorldToCell(collision.transform.position);
            if (MapManager.Instance.GetVisibleTiles().Contains((pos.x, pos.y)))
            {
                AddPopupToScreen();
            }
        }
        else if(isOpen)
        {
            Vector3Int pos = MapDisplay.Instance.WorldToCell(collision.transform.position);
            if (!MapManager.Instance.GetVisibleTiles().Contains((pos.x, pos.y)))
            {
                RemovePopupFromScreen();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isOpen)
        {
            Vector3Int pos = MapDisplay.Instance.WorldToCell(collision.transform.position);
            if (!MapManager.Instance.GetVisibleTiles().Contains((pos.x, pos.y)))
            {
                RemovePopupFromScreen();
            }
        }
    }
}

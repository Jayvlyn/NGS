using System.Collections.Generic;
using UnityEngine;

public class BossfightLevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject startingSection;
    [SerializeField, Tooltip("Prefabs for level sections, index 0 of main array is for entrances on the top, " +
        "continuing clockwise for cardinal directions.")]
    private GameObjectCollection[] levelSections;
    [SerializeField, Tooltip("Prefabs for the end sections, index 0 of main array is for entrances on the top, " +
        "continuing clockwise for cardinal directions")]
    private GameObjectCollection[] endSections;
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private float sizes;
    [SerializeField] private int desiredLevelSections;
    [SerializeField] private bool preventDuplicates = true;


    private void Start()
    {
        int availableSections = 0;
        Dictionary<(int, int), bool> sectionsPlaced = new();
        for(int i = 0; i < levelSections.Length; i++)
        {
            for(int j = 0; j < levelSections[i].gameObjects.Length; j++)
            {
                sectionsPlaced.Add((i, j), false);
                availableSections++;
            }
        }
        availableSections = desiredLevelSections > availableSections && preventDuplicates ? availableSections : desiredLevelSections;
        Dictionary<(int, int), (int, int)> placementLocations = new()
        {
            { (0, 0), (-1, -1) }
        };
        if (GetValidPlacementFrom(ref placementLocations, ref sectionsPlaced, (0, 0), availableSections))
        {
            PlaceAccordingTo(placementLocations);
        }
        else
        {
            Debug.Log("No valid placements found");
        }
    }
    private bool GetValidPlacementFrom(ref Dictionary<(int, int), (int, int)> placementLocations, ref Dictionary<(int, int), bool> sectionsPlaced, (int, int) location, int sectionsLeft)
    {
        int placementDirection = GetEndPosition(placementLocations[location]);
        (int, int) nextPlacement = GetNext(placementDirection, location);
        bool unoccupied = !placementLocations.ContainsKey(nextPlacement);
        bool result = unoccupied && sectionsLeft == 0;
        if(!result && unoccupied)
        {
            int original = Random.Range(0, levelSections[placementDirection].gameObjects.Length);
            int current = original;
            do
            {
                if (!sectionsPlaced[(placementDirection, current)])
                {
                    placementLocations.Add(nextPlacement, (placementDirection, current));
                    sectionsPlaced[(placementDirection, current)] = preventDuplicates;
                    Debug.Log($"Attempted to place section at {nextPlacement}, ");
                    result = GetValidPlacementFrom(ref placementLocations, ref sectionsPlaced, nextPlacement, sectionsLeft - 1);
                    if (!result)
                    {
                        Debug.Log($"Canceled attempt to place section at {nextPlacement}");
                        placementLocations.Remove(nextPlacement);
                        sectionsPlaced[(placementDirection, current)] = false;
                    }
                    else
                    {
                        break;
                    }
                }
                current = (current + 1) % levelSections[placementDirection].gameObjects.Length;
            }
            while (current != original);
        }
        return result;
    }

    private int GetEndPosition((int, int) type)
    {
        return type.Item1 == -1 ? startingSection.GetComponent<BossfightLevelSection>().EndPosition : levelSections[type.Item1].gameObjects[type.Item2].GetComponent<BossfightLevelSection>().EndPosition;
    }

    private void PlaceAccordingTo(Dictionary<(int, int), (int, int)> placementLocations)
    {
        BossfightLevelSection previous = startingSection.GetComponent<BossfightLevelSection>();
        AttemptPlaceEmptyAround(placementLocations, (0, 0));
        (int, int) current = GetNext(previous.EndPosition, (0, 0));
        GameObject[] pastResults = null;
        while(placementLocations.ContainsKey(current))
        {
            (int, int) currentSection = placementLocations[current];
            BossfightLevelSection section = Instantiate(levelSections[currentSection.Item1].gameObjects[currentSection.Item2]).GetComponent<BossfightLevelSection>();
            section.gameObject.transform.position = transform.position + new Vector3(current.Item1 * sizes, current.Item2 * sizes);
            previous.Next = section;
            previous = section;
            pastResults = AttemptPlaceEmptyAround(placementLocations, current);
            current = GetNext(section.EndPosition, current);
        }
        Destroy(pastResults[previous.EndPosition]);
        BossfightLevelSection endSection = Instantiate(endSections[previous.EndPosition].gameObjects[Random.Range(0, endSections[previous.EndPosition].gameObjects.Length)]).GetComponent<BossfightLevelSection>();
        endSection.transform.position = transform.position + new Vector3(current.Item1 * sizes, current.Item2 * sizes);
        previous.Next = endSection;
    }

    private (int, int) GetNext(int direction, (int, int) basedOn)
    {
        (int, int) result = basedOn;
        switch (direction)
        {
            case 1:
                {
                    result.Item1++;
                    break;
                }
            case 2:
                {
                    result.Item2--;
                    break;
                }
            case 3:
                {
                    result.Item1--;
                    break;
                }
            default:
                {
                    result.Item2++;
                    break;
                }
        }
        return result;
    }
    private GameObject[] AttemptPlaceEmptyAround(Dictionary<(int, int), (int, int)> invalidLocations, (int, int)attemptCenter)
    {
        GameObject[] results = new GameObject[4];
        AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 - 1, attemptCenter.Item2 - 1));
        results[3] = AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 - 1, attemptCenter.Item2));
        AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 - 1, attemptCenter.Item2 + 1));
        results[2] = AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1, attemptCenter.Item2 - 1));
        results[0] = AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1, attemptCenter.Item2 + 1));
        AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 + 1, attemptCenter.Item2 - 1));
        results[1] = AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 + 1, attemptCenter.Item2));
        AttemptPlaceEmptyAt(invalidLocations, (attemptCenter.Item1 + 1, attemptCenter.Item2 + 1));
        return results;
    }
    private GameObject AttemptPlaceEmptyAt(Dictionary<(int, int), (int, int)> invalidLocations, (int, int) at)
    {
        GameObject result = null;
        if(!invalidLocations.ContainsKey(at) && (at.Item1 != 0 || at.Item2 != 0))
        {
            result = Instantiate(emptyPrefab);
            result.transform.position = transform.position + transform.position + new Vector3(at.Item1 * sizes, at.Item2 * sizes);
        }
        return result;
    }
}
